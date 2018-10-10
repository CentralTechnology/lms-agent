namespace LMS.Deploy
{
    using Microsoft.Win32;
    using Octokit;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DeploymentOperations
    {
        private readonly string _logFilename;

        private readonly string _setupFilename;

        private readonly bool _localInstall;

        public DeploymentOperations(bool localInstall = false)
        {
            GitHubOperations = new GitHubOperations();
            ServiceOperations = new ServiceOperations();
            _localInstall = localInstall;
            _logFilename = Path.Combine(localInstall ? Directory.GetCurrentDirectory() : Path.GetTempPath(), "LMS.Setup.log");
            _setupFilename = Path.Combine(localInstall ? Directory.GetCurrentDirectory() : Path.GetTempPath(), "LMS.Setup.exe");
        }

        public GitHubOperations GitHubOperations { get; set; }
        public ServiceOperations ServiceOperations { get; set; }

        private static void CopyStream(Stream input, Stream output) => input.CopyTo(output);

        private static SemVer.Version GetCurrentInstalledVersion()
        {
            try
            {
                var x64Info = FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\License Monitoring System\LMS.exe");
                return new SemVer.Version(x64Info.FileMajorPart, x64Info.FileMinorPart, x64Info.FileBuildPart);
            }
            catch (FileNotFoundException ex)
            {
                Log.Debug(ex, ex.Message);
            }

            try
            {
                var x86Info = FileVersionInfo.GetVersionInfo(@"C:\Program Files\License Monitoring System\LMS.exe");
                return new SemVer.Version(x86Info.FileMajorPart, x86Info.FileMinorPart, x86Info.FileBuildPart);
            }
            catch (FileNotFoundException ex)
            {
                Log.Debug(ex, ex.Message);
                Log.Information("Unable to determine the version of the agent installed. Using the default baseline version.");

                return new SemVer.Version(1, 0, 0);
            }
        }

        private static IEnumerable<string> GetQuietUninstallPaths()
        {
            var paths = new List<string>();

            RegistryKey[] keys = new[]
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            };

            foreach (RegistryKey key in keys)
            {
                try
                {
                    foreach (string keyName in key.GetSubKeyNames())
                    {
                        try
                        {
                            RegistryKey subKey = key.OpenSubKey(keyName);
                            string displayName = subKey?.GetValue("DisplayName") as string;
                            if (!string.IsNullOrEmpty(displayName) && displayName.Equals("License Monitoring System"))
                            {
                                string uninstallString = subKey.GetValue("QuietUninstallString") as string;
                                if (!string.IsNullOrEmpty(uninstallString))
                                {
                                    Log.Debug($"Found version {subKey.GetValue("DisplayVersion")}");
                                    Log.Debug($"Uninstall Path: {uninstallString}");
                                    paths.Add(uninstallString);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex, ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, ex.Message);
                }
            }

            return paths;
        }

        private void InstallLatestRelease()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo(_setupFilename, $"/install /quiet /norestart /log {_logFilename}");

                var proc = Process.Start(processStartInfo);
                if (proc == null)
                {
                    throw new NullReferenceException("Unable to launch the installer process");
                }

                proc.WaitForExit();
                Log.Information($"Exit code {proc.ExitCode}");
                if (proc.ExitCode == 0)
                {
                    Log.Information("Successfully updated to the latest version.");
                    return;
                }

                if (proc.ExitCode == 3010)
                {
                    Log.Warning("Successfully updated to the latest version but a reboot is required!");
                    return;
                }

                throw new Exception("Something went wrong during the installation.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public static string GetProgramFilesPath() => Environment.Is64BitOperatingSystem ?
                Environment.GetEnvironmentVariable("ProgramFiles(x86)") :
                Environment.GetEnvironmentVariable("ProgramFiles");

        private void SaveLatestRelease(IApiResponse<byte[]> release)
        {
            using (var streamReader = new MemoryStream(release.Body))
            {
                using (FileStream output = File.OpenWrite(_setupFilename))
                {
                    CopyStream(streamReader, output);
                }
            }
        }

        public async Task StartAsync()
        {
            Release latestRelease = null;
            SemVer.Version latestVersion;
            if (!_localInstall)
            {
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "LMS.Setup.exe")))
                {
                    Log.Debug("Setup file exists. let's go ahead and remove that.");
                    File.Delete("LMS.Setup.exe");
                }

                latestRelease = await GitHubOperations.GetLatestRelease();
                latestVersion = GitHubOperations.GetLatestVersion(latestRelease);
            }
            else
            {
                latestVersion = new SemVer.Version(FileVersionInfo.GetVersionInfo(_setupFilename).ProductVersion);
            }


            SemVer.Version currentVersion = GetCurrentInstalledVersion();

            Log.Information($"Installed: {currentVersion}    Available: {latestVersion}");

            if (currentVersion.CompareTo(latestVersion) < 0)
            {
                if (currentVersion.Equals(new SemVer.Version(1, 0, 0)))
                {
                    Log.Debug("New installation.");
                    if (_localInstall)
                    {
                        await UpdateRequired();
                    }
                    else
                    {
                        await UpdateRequired(latestRelease);
                    }

                    return;
                }

                Log.Debug("Existing installation.");
                Uninstall();
                if (_localInstall)
                {
                    await UpdateRequired();
                }
                else
                {
                    await UpdateRequired(latestRelease);
                }
                return;
            }

            if (currentVersion.CompareTo(latestVersion) > 0)
            {
                Log.Warning("The installed version is newer than what is currently available.");
                Log.Warning("I'm going to uninstall the program.");

                Uninstall();
                if (_localInstall)
                {
                    await UpdateRequired();
                }
                else
                {
                    await UpdateRequired(latestRelease);
                }
                return;
            }

            if (currentVersion.CompareTo(latestVersion) == 0)
            {
                Log.Information("No update is required at this time.");
                Log.Information("Checking installation folder integrity.");
                if (File.Exists(Path.Combine(GetProgramFilesPath(), "License Monitoring System", "LMS.exe")))
                {
                    Log.Information("All looks good!");
                    return;
                }

                Log.Warning("Executable is missing from the installation folder.");
                Log.Warning("Attempting to reinstall the application.");

                Uninstall();
                if (_localInstall)
                {
                    await UpdateRequired();
                }
                else
                {
                    await UpdateRequired(latestRelease);
                }
            }
        }

        private static void Uninstall()
        {
            string[] uninstallPaths = GetQuietUninstallPaths().ToArray();
            if (!uninstallPaths.Any())
            {
                Log.Debug("No uninstall paths found in the registry.");
                return;
            }

            foreach (string uninstallPath in uninstallPaths)
            {
                string[] splitUninstallPath = uninstallPath.Split(new[] { "\" " }, StringSplitOptions.RemoveEmptyEntries);
                string filename = splitUninstallPath[0].TrimStart('\"').Trim();
                string arguments = splitUninstallPath[1].Trim();

                Log.Debug($"Filename: {filename}");
                Log.Debug($"Args: {arguments}");

                if (!File.Exists(filename))
                {
                    Log.Debug("Uninstall file does not exist.");
                    continue;
                }

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = filename,
                        Arguments = arguments
                    }
                };

                try
                {
                    Log.Debug("Starting the uninstall process.");
                    process.Start();
                    process.WaitForExit();
                    Log.Debug($"Exit code {process.ExitCode}");
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, ex.Message);
                    // dont worry about it
                }
            }
        }

        private async Task UpdateRequired()
        {
            Log.Information("Update required!");

            ServiceOperations.Stop();

            Log.Information("Installation started.");
            InstallLatestRelease();

            ServiceOperations.Start();
        }

        private async Task UpdateRequired(Release latestRelease)
        {
            Log.Information("Update required!");

            Log.Information("Getting the download url.");
            string url = await GitHubOperations.GetDownloadUrlAsync(latestRelease);

            Log.Information("Download started. This could take some time...");
            IApiResponse<byte[]> response = await GitHubOperations.DownloadLatestRelease(url);

            Log.Information("Saving file to disk.");
            SaveLatestRelease(response);

            ServiceOperations.Stop();

            Log.Information("Installation started.");
            InstallLatestRelease();

            ServiceOperations.Start();
        }
    }
}