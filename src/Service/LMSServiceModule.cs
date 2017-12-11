namespace LMS.Service
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Abp.Castle.Logging.Log4Net;
    using Abp.Dependency;
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using Castle.Facilities.Logging;
    using global::Hangfire;
    using global::Hangfire.Common;
    using global::Hangfire.Console;
    using global::Hangfire.MemoryStorage;
    using Hangfire;
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

                    // setup event log monitoring
                    ServiceHost.ConfigureEventLog();
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
            IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.LogUsing<Log4NetLoggerFactory>().WithConfig("log4net.config"));

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
}