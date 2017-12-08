namespace LMS.Service
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Abp.Dependency;
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using Castle.Facilities.Logging;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.Console;
    using Hangfire.MemoryStorage;
    using LMS.Startup;
    using Users;
    using Veeam;

    [DependsOn(typeof(LMSEntityFrameworkModule), typeof(AbpHangfireModule))]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LMSServiceModule : AbpModule
    {
        private void ConfigureHangfireJobs()
        {
            var recurringJobManager = new RecurringJobManager();

            recurringJobManager.RemoveIfExists(BackgroundJobNames.Users);
            recurringJobManager.RemoveIfExists(BackgroundJobNames.Veeam);

            using (IDisposableDependencyObjectWrapper<IStartupManager> startupManager = IocManager.ResolveAsDisposable<IStartupManager>())
            {
                if (startupManager.Object.ShouldMonitorUsers(null))
                {
                    recurringJobManager.AddOrUpdate(BackgroundJobNames.Users, Job.FromExpression<UserWorkerManager>(j => j.Start(null)), "*/15 * * * *");
                }

                if (startupManager.Object.MonitorVeeam(null))
                {
                    recurringJobManager.AddOrUpdate(BackgroundJobNames.Veeam, Job.FromExpression<VeeamWorkerManager>(j => j.Start(null)), "*/15 * * * *");
                }
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            ConfigureHangfireJobs();
        }

        public override void PreInitialize()
        {
            IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

            Configuration.BackgroundJobs.UseHangfire(config =>
            {
                config.GlobalConfiguration.UseMemoryStorage();
                config.Server = new BackgroundJobServer(new BackgroundJobServerOptions
                {
                    WorkerCount = 1
                });
                config.GlobalConfiguration.UseConsole();
            });
        }
    }

    public static class BackgroundJobNames
    {
        public const string Users = "Users";
        public const string Veeam = "Veeam";
    }
}