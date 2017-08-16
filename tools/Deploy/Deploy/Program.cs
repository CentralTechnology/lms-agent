using static Deploy.ConsoleExtensions;

namespace Deploy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Common.Extensions;
    using Octokit;

    class Program
    {
        public static GitHubClient Client { get; set; }
        public static string LogFilename { get; set; }

        public static string SetupFilename { get; set; }

        public static void CopyStream(Stream input, Stream output)
        {
            input.CopyTo(output);
        }

        static async Task Install(Release latest)
        {
            int assetId = latest.Assets.Where(x => x.Name == "LMS.Setup.exe").Select(x => x.Id).SingleOrDefault();

            ReleaseAsset asset = await Client.Repository.Release.GetAsset("CentralTechnology", "lms-agent", assetId);

            Information("Downloading the new version");
            try
            {
                IApiResponse<byte[]> response = await Client.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream");

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
                Error("Download failed");
                throw;
            }

            Information("Installing the new version");
            try
            {
                Process proc = Process.Start(SetupFilename, string.Format("/install /quiet /log {0}", LogFilename));
                if (proc == null)
                {
                    throw new NullReferenceException("Unable to launch the installer process");
                }

                proc.WaitForExit();
            }
            catch (Exception)
            {
                Error("Installation failed");
                throw;
            }

            Success("Installation complete");
        }

        static void Main(string[] args)
        {
            Information("Starting deployment....");

            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                Error("************ Deployment  Failed ************");
                throw;
            }

            Success("************ Deployment  Successful ************");
        }

        static async Task MainAsync(string[] args)
        {
            SetupFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");
            LogFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");

            var resourceManager = new ResourceManager("Deploy.Integration", Assembly.GetExecutingAssembly());

            Information("Getting credentials");

            string username = Encoding.UTF8.GetString(Convert.FromBase64String(resourceManager.GetString("GITHUB_USERNAME")));
            if (username == null)
            {
                throw new NullReferenceException("Github username is not set");
            }

            string password = Encoding.UTF8.GetString(Convert.FromBase64String(resourceManager.GetString("GITHUB_PASSWORD")));
            if (password == null)
            {
                throw new NullReferenceException("Github username is not set");
            }

            Client = new GitHubClient(new ProductHeaderValue("lms-deploy"));
            var basicAuth = new Credentials(username, password);
            Client.Credentials = basicAuth;

            Release latest = await Client.Repository.Release.GetLatest("CentralTechnology", "lms-agent");
            Version latestVersion;
            try
            {
                latestVersion = new Version(latest.TagName);
            }
            catch (FormatException)
            {
                throw new FormatException("GitHub tag name is not in the correct format");
            }

            Version installedVersion = CommonExtensions.GetApplicationVersion("License Monitoring System");

            Information(string.Format("Version Installed: {0}", installedVersion));
            Information(string.Format("Version Available: {0}", latestVersion));

            if (installedVersion == null)
            {
                await Install(latest);
                return;
            }

            int versionResult = installedVersion.CompareTo(latestVersion);

            if (versionResult == 0)
            {
                Information("No update required.");
                return;
            }

            if (versionResult < 0)
            {
                Information("Update Required");

                await Install(latest);

                return;
            }

            if (versionResult > 0)
            {
                Information("Somehow the installed version is newer than whats available");
                Information("Can't really do much about that");
            }
        }
    }
}