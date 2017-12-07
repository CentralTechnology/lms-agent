namespace LMS.Service
{
    using System.Reflection;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using Core.Configuration;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.MemoryStorage;
    using LMS.Startup;
    using Users;
    using Veeam;

    [DependsOn(typeof(LMSEntityFrameworkModule), typeof(AbpHangfireModule))]
    public class LMSServiceModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.UseHangfire(config =>
            {
                config.GlobalConfiguration.UseMemoryStorage();
                config.Server = new BackgroundJobServer();
            });
        }

        public override void PostInitialize()
        {
            ConfigureHangfireJobs();
        }

        private void ConfigureHangfireJobs()
        {
            var recurringJobManager = new RecurringJobManager();

            recurringJobManager.RemoveIfExists(BackgroundJobNames.Users);
            recurringJobManager.RemoveIfExists(BackgroundJobNames.Veeam);

            using (var startupManager = IocManager.ResolveAsDisposable<IStartupManager>())
            {
                if (startupManager.Object.MonitorUsers())
                {
                    recurringJobManager.AddOrUpdate(BackgroundJobNames.Users, Job.FromExpression<IUserWorkerManager>(j => j.Start()), "*/15 * * * *");
                }

                if (startupManager.Object.MonitorVeeam())
                {
                    recurringJobManager.AddOrUpdate(BackgroundJobNames.Veeam, Job.FromExpression<IVeeamWorkerManager>(j => j.Start()), "*/15 * * * *");
                }
            }
        }

        
    }

    public class BackgroundJobNames
    {
        public const string Users = "Users";
        public const string Veeam = "Veeam";
    }
}
