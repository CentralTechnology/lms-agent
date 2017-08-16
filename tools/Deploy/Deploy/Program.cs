using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deploy.ConsoleExtensions;

namespace Deploy
{
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using Core.Common.Extensions;
    using Octokit;

    class Program
    {
        static void Main(string[] args)
        {
            Information("Starting deployment....");

            try
            {
                MainAsync(args).GetAwaiter().GetResult();
                Console.ReadLine();
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
            var filename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");
            var logname = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");

            Information("Getting credentials");

            var username = Environment.GetEnvironmentVariable("GITHUB_USERNAME") ?? "centraltechdev";

            if (username == null)
            {
                throw new NullReferenceException("Github username is not set");
            }
            var password = Environment.GetEnvironmentVariable("GITHUB_PASSWORD") ?? "fling-pgP6yH";

            if (password == null)
            {
                throw new NullReferenceException("Github username is not set");
            }

            var client = new GitHubClient(new ProductHeaderValue("lms-deploy"));
            var basicAuth = new Credentials(username, password);
            client.Credentials = basicAuth;

            var latest = await client.Repository.Release.GetLatest("CentralTechnology", "lms-agent");
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

            var versionResult = latestVersion.CompareTo(installedVersion);

            if (versionResult == 0)
            {
                Information("No update required.");
                return;
            }

            if (versionResult > 0)
            {
                Information("Update Required");

                var assetId = latest.Assets.Where(x => x.Name == "LMS.Setup.exe").Select(x => x.Id).SingleOrDefault();

                ReleaseAsset asset = await client.Repository.Release.GetAsset("CentralTechnology", "lms-agent", assetId);

                Information("Downloading the new version");
                try
                {
                    var response = await client.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream");

                    using (var streamReader = new MemoryStream(response.Body))
                    {
                        using (var output = File.OpenWrite(filename))
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
                    var proc = Process.Start(filename, string.Format("/install /quiet /log {0}", logname));
                    proc.WaitForExit();
                }
                catch (Exception)
                {
                    Error("Installation failed");
                    throw;
                }

                Success("Installed complete");

                return;
            }

            if (versionResult < 0)
            {
                Information("Somehow the installed version is newer than whats available");
                Information("Can't really do much about that");
                return;
            }

            
            Console.ReadLine();
        }

        public static void CopyStream(Stream input, Stream output)
        {
            input.CopyTo(output);
        }
    }
}
