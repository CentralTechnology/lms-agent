using static Deploy.ConsoleExtensions;

namespace Deploy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Win32;
    using Nito.AsyncEx;
    using Octokit;
    using Octokit.Internal;
    using Version = SemVer.Version;

    class Program
    {
        static readonly string LogFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.log");

        static readonly string owner = "CentralTechnology";
        static readonly string name = "lms-agent";
        static readonly string SetupFilename = Path.Combine(Path.GetTempPath(), "LMS.Setup.exe");

        private static readonly int MaxRetryAttempts = 3;
        private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(5);

        static InMemoryCredentialStore _credentials;
        private static GitHubClient _client;

        static readonly ResourceManager ResourceManager = new ResourceManager("Deploy.Integration", Assembly.GetExecutingAssembly());

        private static readonly RegistryKey[] UninstallKeys =
        {
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
        };

        public static void CopyStream(Stream input, Stream output)
        {
            input.CopyTo(output);
        }

        static void Deploy()
        {
            Release release = null;
            RetryOnException(MaxRetryAttempts, PauseBetweenFailures, () => { release = AsyncHelper.RunSync(() => _client.Repository.Release.GetLatest(owner, name)); });

            if (release == null)
            {
                Error("Failed to download the latest release information.");
                throw new NullReferenceException();
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
                Information("Unable to determine the version of the agent installed. So i'm going to make one up.");
                installedVersion = new Version("1.0.0");
            }

            Information($"Version Installed: {installedVersion}");
            Information($"Version Available: {latestVersion}");

            if (installedVersion.CompareTo(latestVersion) < 0)
            {
                GetAsset(release);
            }

            if (installedVersion.CompareTo(latestVersion) > 0)
            {
                Information("Somehow the installed version is newer than whats available");
                Information("Can't really do much about that");
                return;
            }

            if (installedVersion.CompareTo(latestVersion) == 0)
            {
                Information("No update required.");
            }
        }

        static System.Version GetApplicationVersion(string pName)
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

        static void GetAsset(Release release)
        {
            int assetId = release.Assets.Where(x => x.Name == "LMS.Setup.exe").Select(x => x.Id).SingleOrDefault();

            ReleaseAsset asset = null;
            RetryOnException(MaxRetryAttempts, PauseBetweenFailures, () => { asset = AsyncHelper.RunSync(() => _client.Repository.Release.GetAsset(owner, name, assetId)); });

            if (asset == null)
            {
                Error("Failed to download the latest asset information.");
                throw new NullReferenceException();
            }

            IApiResponse<byte[]> response = null;

            Information($"Downloading: {release.TagName}");

            RetryOnException(MaxRetryAttempts, PauseBetweenFailures, () => { response = AsyncHelper.RunSync(() => _client.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream")); });

            if (response == null)
            {
                Error("Failed to download the latest asset file.");
                throw new NullReferenceException();
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
                Process proc = Process.Start(SetupFilename, $"/install /quiet /log {LogFilename}");
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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => true;

            string username = Encoding.UTF8.GetString(Convert.FromBase64String(ResourceManager.GetString("GITHUB_USERNAME") ?? throw new InvalidOperationException()));
            if (username == null)
            {
                throw new NullReferenceException("Github username is not set");
            }

            string password = Encoding.UTF8.GetString(Convert.FromBase64String(ResourceManager.GetString("GITHUB_PASSWORD") ?? throw new InvalidOperationException()));
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

        static void RetryOnException(int times, TimeSpan delay, Action operation)
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
                    Information($"Request took: {stopwatch.Elapsed} to complete.");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Error(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Error(ex.InnerException.Message);
                    }
                    if (attempts == times)
                    {
                        throw;
                    }

                    Error($"Exception caught on attempt {attempts} - will retry after delay {delay}");

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
                        Error("Download failed - Cancelled by end user");
                        throw;
                    }

                    stopwatch.Stop();
                    Error($"Time elapsed: {stopwatch.Elapsed}");
                    Error($"Download failed. Attempt: {attempts} - will retry after delay {delay}. Reason: Http Timeout.");
                    Task.Delay(delay).Wait();
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                    if (attempts == times)
                    {
                        throw;
                    }

                    Error($"Exception caught on attempt {attempts} - will retry after delay {delay}");

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }
    }

    public static class RegistryExtensions
    {
        public static (bool exist, string value) GetSubKeyValue(this RegistryKey key, string[] subKeyNames, NameValue filterBy = null, string requestedKeyName = null, string requestedValue = null)
        {
            if (string.IsNullOrEmpty(requestedValue))
            {
                requestedValue = "DisplayName";
            }

            if (requestedKeyName == null)
            {
                foreach (string keyName in subKeyNames)
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey != null)
                    {
                        if (filterBy == null)
                        {
                            if (subkey.GetValue(requestedValue) is string value)
                            {
                                return (true, value);
                            }
                        }
                        else
                        {
                            string filterByValue = subkey.GetValue(filterBy.Name) as string;
                            if (filterBy.Value.Equals(filterByValue, StringComparison.OrdinalIgnoreCase))
                            {
                                if (subkey.GetValue(requestedValue) is string value)
                                {
                                    return (true, value);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (string keyName in subKeyNames.Where(skn => skn.Equals(requestedKeyName, StringComparison.OrdinalIgnoreCase)))
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey?.GetValue(requestedValue) is string value && !string.IsNullOrEmpty(value))
                    {
                        return (true, value);
                    }
                }
            }

            return (false, string.Empty);
        }
    }

    /// <summary>
    ///     Provides some helper methods to work with async methods.
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        ///     Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return method.ReturnType == typeof(Task) ||
                method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        ///     Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        /// <summary>
        ///     Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Can be used to store Name/Value (or Key/Value) pairs.
    /// </summary>
    [Serializable]
    public class NameValue : NameValue<string>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Creates a new <see cref="T:Deploy.NameValue" />.
        /// </summary>
        public NameValue()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates a new <see cref="T:Deploy.NameValue" />.
        /// </summary>
        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    ///     Can be used to store Name/Value (or Key/Value) pairs.
    /// </summary>
    [Serializable]
    public class NameValue<T>
    {
        /// <summary>
        ///     Creates a new <see cref="NameValue" />.
        /// </summary>
        public NameValue()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="NameValue" />.
        /// </summary>
        public NameValue(string name, T value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value.
        /// </summary>
        public T Value { get; set; }
    }
}