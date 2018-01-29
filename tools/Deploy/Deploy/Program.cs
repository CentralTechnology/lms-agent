namespace Deploy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Win32;
    using Octokit;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;
    using Version = SemVer.Version;

    internal class Program
    {
        private const int MaxRetryAttempts = 3;
        private const string Name = "lms-agent";

        private const string Owner = "CentralTechnology";

        private static GitHubClient _client;
        private static readonly string LogFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");
        private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(5);
        private static readonly string SetupFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");

        private static readonly RegistryKey[] UninstallKeys =
        {
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
        };

        private static void CopyStream(Stream input, Stream output)
        {
            input.CopyTo(output);
        }

        private static void Deploy()
        {
            Release release = null;
            RetryOnException(MaxRetryAttempts, PauseBetweenFailures, () => { release = AsyncHelper.RunSync(() => _client.Repository.Release.GetLatest(Owner, Name)); });

            if (release == null)
            {
                throw new NullReferenceException("Failed to download the latest release information.");
            }

            Version latestVersion;
            try
            {
                latestVersion = new Version(release.TagName);
            }
            catch (FormatException)
            {
                throw new FormatException("GitHub tag name is not in the correct format");
            }

            Version installedVersion;
            try
            {
                System.Version appVersion = GetApplicationVersion("License Monitoring System");
                installedVersion = new Version(appVersion.Major, appVersion.Minor, appVersion.Build);
            }
            catch (Exception)
            {
                Log.Information("Unable to determine the version of the agent installed. Using the default baseline version.");
                installedVersion = new Version("1.0.0");
            }

            Log.Information($"Version Installed: {installedVersion}");
            Log.Information($"Version Available: {latestVersion}");

            if (installedVersion.CompareTo(latestVersion) < 0)
            {
                GetAsset(release);
            }

            if (installedVersion.CompareTo(latestVersion) > 0)
            {
                Log.Information("Somehow the installed version is newer than whats available");
                Log.Information("Can't really do much about that");
                return;
            }

            if (installedVersion.CompareTo(latestVersion) == 0)
            {
                Log.Information("No update required.");
            }
        }

        private static System.Version GetApplicationVersion(string pName)
        {
            foreach (RegistryKey key in UninstallKeys)
            {
                if (key != null)
                {
                    (bool exist, string value) data = key.GetSubKeyValue(key.GetSubKeyNames(), new NameValue("DisplayName", pName), requestedValue: "DisplayVersion");
                    if (data.exist)
                    {
                        return new System.Version(data.value);
                    }
                }
            }

            // NOT FOUND
            return null;
        }

        private static void GetAsset(Release release)
        {
            int assetId = release.Assets.Where(x => x.Name == "LMS.Setup.exe").Select(x => x.Id).SingleOrDefault();

            ReleaseAsset asset = null;
            RetryOnException(MaxRetryAttempts, PauseBetweenFailures, () => { asset = AsyncHelper.RunSync(() => _client.Repository.Release.GetAsset(Owner, Name, assetId)); });

            if (asset == null)
            {
                Log.Error("Failed to download the latest assetLog.Information.");
                throw new NullReferenceException();
            }

            IApiResponse<byte[]> response = null;

            Log.Information($"Downloading: {release.TagName}");

            RetryOnException(MaxRetryAttempts, PauseBetweenFailures, () => { response = AsyncHelper.RunSync(() => _client.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream")); });

            if (response == null)
            {
                Log.Error("Failed to download the latest asset file.");
                throw new NullReferenceException();
            }

            Log.Information("Saving the new version");
            try
            {
                using (var streamReader = new MemoryStream(response.Body))
                {
                    using (FileStream output = File.OpenWrite(SetupFilename))
                    {
                        CopyStream(streamReader, output);
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("Saving failed");
                throw;
            }

            Log.Information("Installing the new version");
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(SetupFilename, $"/i /qr /log {LogFilename}");

                Process proc = Process.Start(processStartInfo);
                if (proc == null)
                {
                    throw new NullReferenceException("Unable to launch the installer process");
                }

                proc.WaitForExit();
                Log.Information($"Exit code {proc.ExitCode}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Installation failed.");
                throw;
            }

            Log.Information("Installation successful");
        }

        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Deploy.txt"))
                .CreateLogger();

            Log.Information("Starting deployment....");

            Log.Information("Configuring the GitHub client.");

            ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => true;

            _client = new GitHubClient(new ProductHeaderValue("LMS.Deploy"));
            _client.SetRequestTimeout(TimeSpan.FromMinutes(5));

            try
            {
                Deploy();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Error("************ Deployment  Failed ************");
                throw;
            }

            Log.Information("************ Deployment  Successful ************");
        }

        private static void RetryOnException(int times, TimeSpan delay, Action operation)
        {
            int attempts = 0;
            var stopwatch = new Stopwatch();
            do
            {
                try
                {
                    attempts++;

                    stopwatch.Reset();
                    stopwatch.Start();
                    operation();
                    stopwatch.Stop();
                    Log.Information($"Request took: {stopwatch.Elapsed} to complete.");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Log.Error(ex,ex.Message);
                    if (ex.InnerException != null)
                    {
                        Log.Error(ex.InnerException.Message);
                    }
                    if (attempts == times)
                    {
                        throw;
                    }

                    Log.Error($"Exception caught on attempt {attempts} - will retry after delay {delay}");

                    Task.Delay(delay).Wait();
                }
                catch (TaskCanceledException ex)
                {
                    if (attempts == times)
                    {
                        throw;
                    }

                    if (ex.CancellationToken.IsCancellationRequested)
                    {
                        Log.Error("Download failed - Cancelled by end user");
                        throw;
                    }

                    stopwatch.Stop();
                    Log.Error($"Time elapsed: {stopwatch.Elapsed}");
                    Log.Error($"Download failed. Attempt: {attempts} - will retry after delay {delay}. Reason: Http Timeout.");
                    Log.Error(ex, ex.Message);
                    Task.Delay(delay).Wait();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    if (attempts == times)
                    {
                        throw;
                    }

                    Log.Error($"Exception caught on attempt {attempts} - will retry after delay {delay}");
                    Log.Error(ex, ex.Message);

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }
    }
}