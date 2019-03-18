namespace LMS.Core
{
    using System.Reflection;
    using Abp.Modules;
    using Configuration;
    using Serilog.Core;
    using Services;

    public class LMSCoreModule : AbpModule
    {
        

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