namespace Core.Common.Client.OData
{
    using System;
    using Abp;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Extensions;
    using Factory;
    using Helpers;
    using NLog;
    using Simple.OData.Client;

    public abstract class ODataCommonClientSettings : ODataClientSettings
    {
        protected ODataCommonClientSettings()
        {
            Logger = LogManager.GetCurrentClassLogger();
            SettingManager = SettingFactory.SettingsManager();

            RenewHttpConnection = true;
        }

        protected static Guid DeviceId { get; set; }
        public Logger Logger { get; set; }

        public SettingManager SettingManager { get; set; }

        protected static string Token { get; set; }

        protected void ValidateDeviceId()
        {
            if (DeviceId == default(Guid))
            {
                DeviceId = SettingManagerHelper.DeviceId;
            }
        }

        protected void ValidateToken()
        {
            if (Token.IsNullOrEmpty())
            {
                Token = AsyncHelper.RunSync(() => ClientFactory.PortalClient().GetTokenCookie());
            }
        }
    }
}