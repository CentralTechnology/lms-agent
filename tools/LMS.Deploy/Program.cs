namespace Deploy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Octokit;
    using Polly;
    using Serilog;
    using Version = SemVer.Version;

    class Program
    {
        private const string Name = "lms-agent";

        private const string Owner = "CentralTechnology";

        private static readonly string LogFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");
        private static readonly string SetupFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");

        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                .WriteTo.File("deploy.txt", Serilog.Events.LogEventLevel.Debug)
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

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Press [Enter] to exit.");
                Console.ReadLine();
            }
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

        public static Version GetApplicationVersion(string pName)
        {
            try
            {
                FileVersionInfo x64Info = FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\License Monitoring System\LMS.exe");
                return new Version(x64Info.FileMajorPart,x64Info.FileMinorPart,x64Info.FileBuildPart);
            }
            catch (FileNotFoundException)
            {

            }

            try
            {
                FileVersionInfo x86Info = FileVersionInfo.GetVersionInfo(@"C:\Program Files\License Monitoring System\LMS.exe");
                return new Version(x86Info.FileMajorPart,x86Info.FileMinorPart,x86Info.FileBuildPart);
            }
            catch (FileNotFoundException)
            {
                return new Version(1, 0, 0);
            }
        }

        public static Version GetCurrentInstalledVersion()
        {
            try
            {
                return GetApplicationVersion("License Monitoring System");
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
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(),"LMS.Setup.exe")))
            {
                Log.Debug("Setup file exists. let's go ahead and remove that.");
                File.Delete("LMS.Setup.exe");
            }
            
            var latestRelease = await GetLatestRelease(client);
            var latestVersion = GetLatestVersion(latestRelease);
            var currentVersion = GetCurrentInstalledVersion();

            Log.Information($"Installed: {currentVersion}    Available: {latestVersion}");

            if (currentVersion.CompareTo(latestVersion) < 0)
            {
                if (File.Exists(Path.Combine(ProgramFilesx86(), "License Monitoring System", "LMS.exe")))
                {
                    await UpdateRequired(client, latestRelease);
                    return;
                }

                Log.Warning("Oh dear me, the executable does not appear to exist, even though the application is installed!");
                Log.Warning("Let's uninstall it and see what happens.");
                Uninstall();
                await UpdateRequired(client, latestRelease);
            }

            if (currentVersion.CompareTo(latestVersion) > 0)
            {
                Log.Warning("The installed version is newer than what is currently available.");
                Log.Warning("i'm going to uninstall the program.");
                Uninstall();
                await UpdateRequired(client, latestRelease);
                return;
            }

            if (currentVersion.CompareTo(latestVersion) == 0)
            {
                Log.Information("No update is required at this time, but let's just make sure all the files are there.");
                if (File.Exists(Path.Combine(ProgramFilesx86(), "License Monitoring System", "LMS.exe")))
                {
                    Log.Information("All looks good!");
                    return;
                }

                Log.Warning("Oh dear me, the executable does not appear to exist, even though the application is installed!");
                Log.Warning("Let's uninstall it and see what happens.");
                Uninstall();
                await UpdateRequired(client, latestRelease);
            }
        }

        public static void Uninstall()
        {
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = @"C:\ProgramData\Package Cache\{097afb2c-3985-4c01-8169-57ee425edc4b}\LMS.Setup.exe",
                    Arguments = "/uninstall /quiet"
                }
            };
            process.Start();
            process.WaitForExit();
        }

       public static async Task UpdateRequired(GitHubClient client, Release latestRelease)
        {
                Log.Information("Update required!");

                Log.Information("Getting the download url.");
                var url = await GetDownloadUrlAsync(client, latestRelease);

                Log.Information("Download started. This could take some time...");
                var response = await DownloadLatestRelease(client, url);

                Log.Information("Saving file to disk.");
                SaveLatestRelease(response);

                Log.Information("Making sure the service is stopped.");
                try
                {
                    ServiceController service = new ServiceController("LicenseMonitoringSystem");
                    Log.Information($"Service is currently {service.Status}");
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        Log.Information("Attempting to stop the License Monitoring System service.");
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));

                        if (service.Status != ServiceControllerStatus.Stopped)
                        {
                            Log.Information("Service has failed to stop in a reasonable amount of time.");
                            Log.Information("Killing the process");
                            foreach (var process in Process.GetProcessesByName("LMS"))
                            {
                                process.Kill();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("License Monitoring System service was not found on this computer.");
                    Log.Debug(ex, ex.Message);
                }

                Log.Information("Installation started.");
                InstallLatestRelease();

                Log.Information("Making sure the service is started.");
                try
                {
                    ServiceController service = new ServiceController("LicenseMonitoringSystem");
                    Log.Information($"Service is currently {service.Status}");
                    if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
                    {
                        Log.Information("Attempting to start the License Monitoring System service.");
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                        Log.Information("Service successfully started.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("License Monitoring System service was not found on this computer.");
                    Log.Debug(ex, ex.Message);
                }
        }

        static string ProgramFilesx86()
        {
            if( 8 == IntPtr.Size 
                || (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
    }
}