namespace Core.Common.Client.OData
{
    using System;
    using Abp;
    using Abp.Dependency;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Factory;
    using NLog;
    using Simple.OData.Client;

    public abstract class ODataCommonClientSettings : ODataClientSettings
    {
        protected ODataCommonClientSettings()
        {
            Logger= LogManager.GetCurrentClassLogger();
            SettingsManager = SettingFactory.SettingsManager();
        }

        public SettingsManager SettingsManager { get; set; }
        public Logger Logger { get; set; }

        protected static Guid DeviceId { get; set; }

        protected static string Token { get; set; }

        protected void ValidateDeviceId()
        {

                if (DeviceId == Guid.Empty)
                {
                    DeviceId = SettingsManager.Read().DeviceId;

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