namespace LicenseMonitoringSystem.Tests
{
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using LMS;
    using LMS.Core;

    [DependsOn(
        typeof(LMSCoreModule),
        typeof(AbpHangfireModule))]
    public class LicenseMonitoringSystemTestModule : AbpModule
    {
        public override void PostInitialize()
        {
            Configuration.BackgroundJobs.UseHangfire(config =>
            {
                config.GlobalConfiguration.UseMemoryStorage();
                config.Server = new BackgroundJobServer(new BackgroundJobServerOptions
                {
                    WorkerCount = 1
                });
            });

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }
    }
}