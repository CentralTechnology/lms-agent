namespace LMS
{
    using System.Reflection;
    using Abp.Modules;
    using Core;

    [DependsOn(typeof(LMSCoreModule), typeof(LMSEntityFrameworkModule))]
    public class LMSServiceModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
