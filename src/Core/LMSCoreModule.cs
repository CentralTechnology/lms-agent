namespace LMS
{
    using System.Reflection;
    using Abp.AutoMapper;
    using Abp.Modules;
    using AutoMapper;
    using Common.Extensions;
    using Configuration;
    using Core.Services;
    using Serilog.Core;

    [DependsOn(typeof(AbpAutoMapperModule))]
    public class LMSCoreModule : AbpModule
    {
        public static LoggingLevelSwitch CurrentLogLevel {get;set;}
        public override void PreInitialize()
        {
            Configuration.MultiTenancy.IsEnabled = false;
            Configuration.Settings.Providers.Add<AppSettingProvider>();   
        }
        public override void Initialize()
        {
            IocManager.Register<IPortalService, PortalService>();

            var thisAssembly = typeof(LMSCoreModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.AddProfiles(thisAssembly);
            });
        }

        public override void PostInitialize() => Mapper.AssertConfigurationIsValid();
    }
}
