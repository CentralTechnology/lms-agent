namespace LMS.Core.Services.Authentication
{
    using System;
    using System.Net;
    using System.Threading;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.UI;
    using Clients;
    using Configuration;
    using Extensions;
    using Helpers;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using RestSharp;
    using Serilog;

    public sealed class PortalAuthenticationService
    {
        private static readonly Lazy<PortalAuthenticationService> _instance =
            new Lazy<PortalAuthenticationService>(() => new PortalAuthenticationService());

        private static SettingManager _settingManager;

        private static PortalToken _token;

        private static readonly ILogger Logger = Log.ForContext<PortalAuthenticationService>();

        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);

        private PortalAuthenticationService()
        {
            _settingManager = IocManager.Instance.Resolve<SettingManager>();
        }

        public static PortalAuthenticationService Instance => _instance.Value;

        public string GetAccessToken()
        {
            _tokenLock.Wait();

            try
            {
                if (_token == null)
                {
                    dynamic token = RequestToken();
                    _token = PortalToken.Create(token.access_token, token.expires_in, token.request_time);
                    return _token.AccessToken;
                }

                if (_token.IsNearExpiry)
                {
                    dynamic token = RequestToken();
                    _token.Update(token.access_token, token.expires_in, token.request_time);
                    return _token.AccessToken;
                }

                return _token.AccessToken;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        public long GetAccount()
        {
            Guid device = GetDevice();
            return GetAccount(device);
        }

        public long GetAccount(Guid device)
        {
            string account = _settingManager.GetSettingValue(AppSettingNames.AutotaskAccountId);
            if (string.IsNullOrEmpty(account))
            {
                Logger.Information("Account number not found in the local database.");

                long idFromService = GetAccountFromService(device);
                _settingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, idFromService.ToString());
                return idFromService;
            }

            if (long.TryParse(account, out long accountId))
            {
                if (accountId != default(long))
                {
                    return accountId;
                }
            }

            var id = GetAccountFromService(device);
            _settingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, id.ToString());
            return id;
        }

        private long GetAccountFromService(Guid device)
        {
            Logger.Information("Requesting Account number from the api.");

            var client = new RestClientBase(GetAccountUri());
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Device", device);

            try
            {
                return Convert.ToInt64(client.Execute(request).Content);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw new UserFriendlyException("Unable to get the Account number from the api. Please manually enter the Account number through the CLI.");
            }
        }

        private static string GetAccountUri()
        {
            return DebuggingService.Debug ? "http://localhost:64755/auth/account" : "https://api-v2.portal.ct.co.uk/auth/account";
        }

        public Guid GetDevice()
        {
            string device = _settingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId);
            if (string.IsNullOrEmpty(device))
            {
                Logger.Information("Account identifier not found in the local database.");

                Guid deviceFromRegistry = GetDeviceFromRegistry();
                _settingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, deviceFromRegistry.ToString());
                return deviceFromRegistry;
            }

            if (Guid.TryParse(device, out Guid deviceId))
            {
                if (deviceId != default(Guid))
                {
                    return deviceId;
                }
            }

            Guid id = GetDeviceFromRegistry();
            _settingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, id.ToString());
            return id;
        }

        private static Guid GetDeviceFromRegistry()
        {
            if (DebuggingService.Debug)
            {
                return new Guid("2a5d23dc-1b9a-9341-32c6-1160a5df7883");
            }

            var keys = new[]
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE")
            };

            foreach (RegistryKey key in keys)
            {
                try
                {
                    (bool exist, string value) data = key.GetSubKeyValue(key.GetSubKeyNames(), requestedKeyName: "CentraStage", requestedValue: "DeviceID");
                    if (data.exist)
                    {
                        bool valid = Guid.TryParse(data.value, out Guid csId);
                        if (valid)
                        {
                            return csId;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                }
            }

            throw new UserFriendlyException("Unable to get the Device identifier from the registry. Please manually enter the Device identifier through the CLI.");
        }

        private static string GetTokenUri()
        {
            return DebuggingService.Debug ? "http://localhost:64755/auth/token" : "https://api-v2.portal.ct.co.uk/auth/token";
        }

        private dynamic RequestToken()
        {
            long account = GetAccount();
            Guid device = GetDevice();

            var client = new RestClientBase(GetTokenUri());
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Account", account);
            request.AddParameter("Device", device);

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new AuthenticationException(string.Format("Authentication Failed: {0}", response.Content), response.ErrorException);
            }

            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
    }
}