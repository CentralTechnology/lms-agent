namespace LicenseMonitoringSystem.Tests
{
    using System.Data.Entity;
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using Abp.TestBase;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using LMS;
    using LMS.EntityFramework;
    using LMS.Service;

    [DependsOn(
        typeof(LMSCoreModule),
        typeof(LMSEntityFrameworkModule),
        typeof(AbpTestBaseModule),
        typeof(AbpHangfireModule))]
    public class LicenseMonitoringSystemTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<LMSDbContext>());
        }
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