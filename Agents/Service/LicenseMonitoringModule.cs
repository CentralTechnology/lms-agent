namespace LicenseMonitoringSystem
{
    using System.Reflection;
    using Abp.Modules;
    using Core;

    [DependsOn(typeof(LicenseMonitoringCoreModule))]
    public class LicenseMonitoringModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}