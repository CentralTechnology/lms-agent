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
    using Abp.Threading;
    using Core.Common.Extensions;
    using Octokit;
    using Octokit.Internal;

    class Program
    {
        static readonly string LogFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");

        static readonly string owner = "CentralTechnology";
        static readonly string name = "lms-agent";
        static readonly string SetupFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");

        static InMemoryCredentialStore _credentials;
        private static GitHubClient _client;

        static readonly ResourceManager ResourceManager = new ResourceManager("Deploy.Integration", Assembly.GetExecutingAssembly());

        public static void CopyStream(Stream input, Stream output)
        {
            input.CopyTo(output);
        }

        static void Deploy()
        {
            Release release = AsyncHelper.RunSync(() => _client.Repository.Release.GetLatest(owner, name));

            Version latestVersion;
            try
            {
                latestVersion = new Version(release.TagName);
            }
            catch (FormatException)
            {
                throw new FormatException("GitHub tag name is not in the correct format");
            }

            Version installedVersion = CommonExtensions.GetApplicationVersion("License Monitoring System");

            Information(string.Format("Version Installed: {0}", installedVersion));
            Information(string.Format("Version Available: {0}", latestVersion));

            if (installedVersion == null || installedVersion.CompareTo(latestVersion) < 0)
            {
                GetAsset(release);
            }

            if (installedVersion != null && installedVersion.CompareTo(latestVersion) > 0)
            {
                Information("Somehow the installed version is newer than whats available");
                Information("Can't really do much about that");
                return;
            }

            if (installedVersion != null && installedVersion.CompareTo(latestVersion) == 0)
            {
                Information("No update required.");
            }
        }

        static void GetAsset(Release release)
        {
            int assetId = release.Assets.Where(x => x.Name == "LMS.Setup.exe").Select(x => x.Id).SingleOrDefault();

            ReleaseAsset asset = AsyncHelper.RunSync(() => _client.Repository.Release.GetAsset(owner, name, assetId));

            IApiResponse<byte[]> response;
            try
            {
                Information(string.Format("Downloading: {0}", release.TagName));
                response = AsyncHelper.RunSync(() => _client.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream"));
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    Error("Download failed - Cancelled by end user");
                }
                else
                {
                    Error("Download failed - Http Timeout (most likely)");
                }

                throw;
            }

            Information("Saving the new version");
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
                Error("Saving failed");
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

            Information("Getting credentials");

            string username = Encoding.UTF8.GetString(Convert.FromBase64String(ResourceManager.GetString("GITHUB_USERNAME")));
            if (username == null)
            {
                throw new NullReferenceException("Github username is not set");
            }

            string password = Encoding.UTF8.GetString(Convert.FromBase64String(ResourceManager.GetString("GITHUB_PASSWORD")));
            if (password == null)
            {
                throw new NullReferenceException("Github username is not set");
            }

            _credentials = new InMemoryCredentialStore(new Credentials(username, password));
            _client = new GitHubClient(new ProductHeaderValue("LMS.Deploy"), _credentials);

            try
            {
                Deploy();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                Error("************ Deployment  Failed ************");
                throw;
            }

            Success("************ Deployment  Successful ************");
        }
    }
}