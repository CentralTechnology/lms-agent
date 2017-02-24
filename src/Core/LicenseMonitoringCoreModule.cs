namespace Core
{
    using System.Reflection;
    using Abp.AutoMapper;
    using Abp.Dependency;
    using Abp.Modules;
    using AutoMapper;
    using Common.Client;

    public class LicenseMonitoringCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

        }
    }
}