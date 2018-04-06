using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services.Authentication
{
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.UI;
    using Common.Managers;
    using Configuration;
    using LMS.Common.Extensions;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using RestSharp;

    public class PortalAuthenticationService : LMSManagerBase, IPortalAuthenticationService, ISingletonDependency
    {
#if DEBUG
        private const string AccountUri = "http://localhost:64755/auth/account";
        private const string TokenUri = "http://localhost:64755/auth/token";
#else
        private const string AccountUri = "https://api-v1.portal.ct.co.uk/auth/account";
    private const string TokenUri = "https://api-v1.portal.ct.co.uk/auth/token";
#endif

        private readonly object _tokenLock = new object();
        private PortalToken Token { get; set; }

        public string GetToken()
        {
            lock (_tokenLock)
            {
                if (Token == null)
                {
                    Token = RequestToken();
                }

                if (Token.IsNearExpiry())
                {
                    Token = RequestToken();
                }

                return Token.access_token;
            }
        }

        public long GetAccount()
        {
            Guid device = GetDevice();
            return GetAccount(device);
        }

        public long GetAccount(Guid device)
        {
            string account = SettingManager.GetSettingValue(AppSettingNames.AutotaskAccountId);
            if (string.IsNullOrEmpty(account))
            {
                Logger.Info("Account number not found in the local database.");

                long idFromService = GetAccountFromService(device);
                SettingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, idFromService.ToString());
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
            SettingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, id.ToString());
            return id;
        }

        private long GetAccountFromService(Guid device)
        {
            Logger.Info("Requesting Account number from the api.");

            var client = new RestClient(AccountUri);
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

        public Guid GetDevice()
        {
            string device = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId);
            if (string.IsNullOrEmpty(device))
            {
                Logger.Info("Account identifier not found in the local database.");

                Guid deviceFromRegistry = GetDeviceFromRegistry();
                SettingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, deviceFromRegistry.ToString());
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
            SettingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, id.ToString());
            return id;
        }

        private Guid GetDeviceFromRegistry()
        {
#if DEBUG
            return new Guid("2a5d23dc-1b9a-9341-32c6-1160a5df7883");
#endif
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

        private PortalToken RequestToken()
        {
            long account = GetAccount();
            Guid device = GetDevice();

            var client = new RestClient(TokenUri);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Account", account);
            request.AddParameter("Device", device);

            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<PortalToken>(response.Content, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });
        }
    }
}
