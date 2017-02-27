namespace Core
{
    using System.Reflection;
    using Abp.Modules;
    using Abp.WebApi;
    using Common.Client;

    [DependsOn(typeof(AbpWebApiModule))]
    public class LicenseMonitoringCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}