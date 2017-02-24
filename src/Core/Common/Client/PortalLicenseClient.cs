namespace Core.Common.Client
{
    using Abp;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Settings;
    using Simple.OData.Client;

    public class PortalLicenseClient : ODataClient, ITransientDependency
    {
        public PortalLicenseClient()
            : base(new DefaultLicenseClientSettings())
        {
            SettingManager = IocManager.Instance.Resolve<ISettingManager>();
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public ISettingManager SettingManager { get; set; }
    }

    public class DefaultLicenseClientSettings : ODataClientSettings, ITransientDependency
    {
        public DefaultLicenseClientSettings()
        {
            ISettingManager settingManager = IocManager.Instance.Resolve<ISettingManager>();

            BeforeRequest += br => { br.Headers.Add("Authorization", $"Device {settingManager.GetDeviceId()}"); };

            if (settingManager.GetDebug())
            {
                OnTrace += (x, y) => { Logger.Debug($"{x} {y}"); };
            }
        }

        public ILogger Logger { get; set; }
    }
}