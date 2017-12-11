namespace LMS
{
    using System.Reflection;
    using Abp.AutoMapper;
    using Abp.Modules;
    using Abp.WebApi;
    using Common.Extensions;
    using Configuration;

    [DependsOn(typeof(AbpWebApiModule), typeof(AbpAutoMapperModule))]
    public class LMSCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.MultiTenancy.IsEnabled = false;
            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
        }
        public override void Initialize()
        {
            var thisAssembly = typeof(LMSCoreModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.AddProfiles(thisAssembly);
            });
        }
    }
}
