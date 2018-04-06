namespace LMS
{
    using System.Reflection;
    using Abp.Modules;
    using Configuration;
    using Core.Services;
    using Serilog.Core;

    public class LMSCoreModule : AbpModule
    {
        public static LoggingLevelSwitch CurrentLogLevel { get; set; }

        public override void Initialize()
        {
            IocManager.Register<IPortalService, PortalService>();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PreInitialize()
        {
            Configuration.MultiTenancy.IsEnabled = false;
            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }
    }
}