namespace LMS.Deploy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;
    using Polly;
    using Serilog;

    public class GitHubOperations
    {
        private const string Name = "lms-agent";

        private const string Owner = "CentralTechnology";

        public GitHubOperations()
        {
            Client = new GitHubClient(new ProductHeaderValue("LMS.Deploy"));
            Client.SetRequestTimeout(TimeSpan.FromMinutes(30));
        }

        public GitHubClient Client { get; set; }

        public async Task<IApiResponse<byte[]>> DownloadLatestRelease(string url)
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
                        Log.Debug(exception, exception.Message);
                    });

            var response = await policy.ExecuteAsync(() => Client.Connection.Get<byte[]>(new Uri(url), new Dictionary<string, string>(), "application/octet-stream"));
            if (response == null)
            {
                throw new Exception("Failed to download the latest release!");
            }

            return response;
        }

        public async Task<string> GetDownloadUrlAsync(Release release)
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
                        Log.Debug(exception, exception.Message);
                    });

            var asset = await policy.ExecuteAsync(() => Client.Repository.Release.GetAsset(Owner, Name, release.Assets.Where(a => a.Name == "LMS.Setup.exe").Select(a => a.Id).Single()));
            if (asset == null)
            {
                throw new Exception("Failed to get the latest release download url!");
            }

            return asset.Url;
        }

        public async Task<Release> GetLatestRelease()
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
                        Log.Debug(exception, exception.Message);
                    });

            Release release = await policy.ExecuteAsync(() => Client.Repository.Release.GetLatest(Owner, Name));
            if (release == null)
            {
                throw new Exception("Unable to download the latest release");
            }

            return release;
        }

        public SemVer.Version GetLatestVersion(Release release)
        {
            if (release == null)
            {
                throw new Exception("Cannot determine the latest version if the release is empty!");
            }

            try
            {
                return new SemVer.Version(release.TagName);
            }
            catch (FormatException ex)
            {
                Log.Error(ex, ex.Message);
                throw new FormatException("GitHub tag name is not in the correct format");
            }
        }
    }
}