namespace Deploy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Win32;
    using Octokit;
    using Polly;
    using Serilog;
    using Version = SemVer.Version;

    public class Program
    {
        private const string Name = "lms-agent";

        private const string Owner = "CentralTechnology";

        private static readonly string LogFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");
        private static readonly string SetupFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");

        private static readonly RegistryKey[] UninstallKeys =
        {
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
        };

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("deploy.txt")
                .CreateLogger();

            Log.Information("Starting deployment....");

            Log.Information("Building the GitHub client.");

            var client = BuildClient();
            try
            {
                await StartDeployment(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Error("************ Deployment  Failed ************");
                throw;
            }

            Log.Information("************ Deployment  Successful ************");
        }

        public static GitHubClient BuildClient()
        {
            var client = new GitHubClient(new ProductHeaderValue("LMS.Deploy"));
            client.SetRequestTimeout(TimeSpan.FromMinutes(10));
            return client;
        }

        private static void CopyStream(Stream input, Stream output) => input.CopyTo(output);

        public static async Task<IApiResponse<byte[]>> DownloadLatestRelease(GitHubClient client, string url)
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(6, // We can also do this with WaitAndRetryForever... but chose WaitAndRetry this time.
                    attempt => TimeSpan.FromSeconds(0.1 * Math.Pow(2, attempt)), // Back off!  2, 4, 8, 16 etc times 1/4-second
                    (exception, calculatedWaitDuration) => // Capture some info for logging!
                    {
                        // This is your new exception handler! 
                        // Tell the user what they've won!
                        Log.Warning("Exception: " + exception.Message);
                        Log.Warning(" ... automatically delaying for " + calculatedWaitDuration.TotalMilliseconds + "ms.");
                    });

            var response = await policy.ExecuteAsync(() => client.Connection.Get<byte[]>(new Uri(url), new Dictionary<string, string>(), "application/octet-stream"));
            if (response == null)
            {
                throw new Exception("Failed to download the latest release!");
            }

            return response;
        }

        public static System.Version GetApplicationVersion(string pName)
        {
            foreach (RegistryKey key in UninstallKeys)
            {
                if (key != null)
                {
                    (bool exist, string value) = key.GetSubKeyValue(key.GetSubKeyNames(), new NameValue("DisplayName", pName), requestedValue: "DisplayVersion");
                    if (exist)
                    {
                        return new System.Version(value);
                    }
                }
            }

            // NOT FOUND
            return null;
        }

        public static Version GetCurrentInstalledVersion()
        {
            try
            {
                System.Version appVersion = GetApplicationVersion("License Monitoring System");
                return new Version(appVersion.Major, appVersion.Minor, appVersion.Build);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Information("Unable to determine the version of the agent installed. Using the default baseline version.");
                return new Version("1.0.0");
            }
        }

        public static async Task<string> GetDownloadUrlAsync(GitHubClient client, Release release)
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(6, // We can also do this with WaitAndRetryForever... but chose WaitAndRetry this time.
                    attempt => TimeSpan.FromSeconds(0.1 * Math.Pow(2, attempt)), // Back off!  2, 4, 8, 16 etc times 1/4-second
                    (exception, calculatedWaitDuration) => // Capture some info for logging!
                    {
                        // This is your new exception handler! 
                        // Tell the user what they've won!
                        Log.Warning("Exception: " + exception.Message);
                        Log.Warning(" ... automatically delaying for " + calculatedWaitDuration.TotalMilliseconds + "ms.");
                    });

            var asset = await policy.ExecuteAsync(() => client.Repository.Release.GetAsset(Owner, Name, release.Assets.Where(a => a.Name == "LMS.Setup.exe").Select(a => a.Id).Single()));
            if (asset == null)
            {
                throw new Exception("Failed to get the latest release download url!");
            }

            return asset.Url;
        }

        public static async Task<Release> GetLatestRelease(GitHubClient client)
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(6, // We can also do this with WaitAndRetryForever... but chose WaitAndRetry this time.
                    attempt => TimeSpan.FromSeconds(0.1 * Math.Pow(2, attempt)), // Back off!  2, 4, 8, 16 etc times 1/4-second
                    (exception, calculatedWaitDuration) => // Capture some info for logging!
                    {
                        // This is your new exception handler! 
                        // Tell the user what they've won!
                        Log.Warning("Exception: " + exception.Message);
                        Log.Warning(" ... automatically delaying for " + calculatedWaitDuration.TotalMilliseconds + "ms.");
                    });

            Release release = await policy.ExecuteAsync(() => client.Repository.Release.GetLatest(Owner, Name));
            if (release == null)
            {
                throw new Exception("Unable to download the latest release");
            }

            return release;
        }

        public static Version GetLatestVersion(Release release)
        {
            if (release == null)
            {
                throw new Exception("Cannot determine the latest version if the release is empty!");
            }

            try
            {
                return new Version(release.TagName);
            }
            catch (FormatException ex)
            {
                Log.Error(ex, ex.Message);
                throw new FormatException("GitHub tag name is not in the correct format");
            }
        }

        public static void InstallLatestRelease()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(SetupFilename, $"/install /quiet /norestart /log {LogFilename}");

                Process proc = Process.Start(processStartInfo);
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

                throw new Exception("Something went wrong during the installation.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public static void SaveLatestRelease(IApiResponse<byte[]> release)
        {
            using (var streamReader = new MemoryStream(release.Body))
            {
                using (FileStream output = File.OpenWrite(SetupFilename))
                {
                    CopyStream(streamReader, output);
                }
            }
        }

        public static async Task StartDeployment(GitHubClient client)
        {
            var latestRelease = await GetLatestRelease(client);
            var latestVersion = GetLatestVersion(latestRelease);
            var currentVersion = GetCurrentInstalledVersion();

            Log.Information($"Installed: {currentVersion}    Available: {latestVersion}");

            if (currentVersion.CompareTo(latestVersion) < 0)
            {
                Log.Information("Update required!");

                Log.Information("Getting the download url.");
                var url = await GetDownloadUrlAsync(client, latestRelease);

                Log.Information("Download started. This could take some time...");
                var response = await DownloadLatestRelease(client, url);

                Log.Information("Saving file to disk.");
                SaveLatestRelease(response);

                Log.Information("Installation started.");
                InstallLatestRelease();
            }

            if (currentVersion.CompareTo(latestVersion) > 0)
            {
                Log.Error("The installed version is newer than what is currently available.");
                Log.Error("Please uninstall the current version and rerun the deploy utility.");
                throw new Exception("Installed version is newer than what is currently available.");
            }

            if (currentVersion.CompareTo(latestVersion) == 0)
            {
                Log.Information("No update is required at this time.");
            }
        }
    }
}