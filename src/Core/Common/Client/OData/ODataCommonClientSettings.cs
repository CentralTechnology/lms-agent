namespace Core.Common.Client.OData
{
    using System;
    using Abp;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Extensions;
    using Factory;
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
            if (DeviceId == Guid.Empty)
            {
                DeviceId = SettingManager.GetSettingValue<Guid>(SettingNames.CentrastageDeviceId);

                if (DeviceId == Guid.Empty)
                {
                    throw new AbpException($"Cannot perform web request when device id is {Guid.Empty}");
                }
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