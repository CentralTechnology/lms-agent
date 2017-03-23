namespace Core
{
    using Abp.Dependency;
    using Administration;
    using Castle.Core.Logging;

    public abstract class LicenseMonitoringBase : ITransientDependency
    {
        protected LicenseMonitoringBase()
        {
            Logger = NullLogger.Instance;
            SettingManager = IocManager.Instance.Resolve<ISettingsManager>();
        }

        public ILogger Logger { get; set; }
        public ISettingsManager SettingManager { get; set; }
    }
}