namespace Core.Common.Helpers
{
    using System;
    using System.Reflection;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Client;
    using Extensions;
    using NLog;

    public class SettingManagerHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly SettingManager SettingManager = new SettingManager();
        private static int _accountId;

        private static Guid _deviceId;

        private static string _token;

        public static int AccountId
        {
            get
            {
                if (_accountId == default(int))
                {
                    int dbLookup = SettingManager.GetSettingValue<int>(SettingNames.AutotaskAccountId);

                    _accountId = dbLookup;
                }

                return _accountId;
            }
        }

        public static string ClientVersion
        {
            get
            {
                try
                {
                    return Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                catch (Exception ex)
                {
                    Logger.Error("Unable to determine client version.");
                    Logger.Debug(ex);
                }

                return string.Empty;
            }
        }

        public static Guid DeviceId
        {
            get
            {
                if (_deviceId == default(Guid))
                {
                    var dbLookup = SettingManager.GetSettingValue<Guid>(SettingNames.CentrastageDeviceId);

                    _deviceId = dbLookup;
                }

                return _deviceId;
            }
        }

        public static string Token
        {
            get
            {
                if (_token.IsNullOrEmpty())
                {
                    _token = AsyncHelper.RunSync(() => new PortalWebApiClient().GetTokenCookie());
                }

                return _token;
            }
        }
    }
}