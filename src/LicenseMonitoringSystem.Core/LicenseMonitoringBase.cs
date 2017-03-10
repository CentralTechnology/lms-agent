namespace Core
{
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Settings;

    public abstract class LicenseMonitoringBase : ITransientDependency
    {
        protected LicenseMonitoringBase()
        {
            Logger = NullLogger.Instance;
            SettingManager = IocManager.Instance.Resolve<ISettingManager>();
        }

        public ILogger Logger { get; set; }
        public ISettingManager SettingManager { get; set; }
    }
}