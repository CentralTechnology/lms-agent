namespace LicenseMonitoringSystem.Core
{
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Settings;

    public abstract class LicenseMonitoringBase : ITransientDependency
    {
        protected LicenseMonitoringBase(SettingManager settingManager)
        {
            Logger = NullLogger.Instance;
            SettingManager = settingManager;
        }

        public ILogger Logger { get; set; }
        public SettingManager SettingManager { get; set; }
    }
}