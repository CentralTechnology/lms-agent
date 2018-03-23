namespace LMS.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Abp.Castle.Logging.Log4Net;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using Castle.Facilities.Logging;
    using Configuration;
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
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PreInitialize()
        {
            IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.LogUsing<Log4NetLoggerFactory>().WithConfig("log4net.config"));

            Configuration.BackgroundJobs.UseHangfire(config =>
            {
                config.GlobalConfiguration.UseMemoryStorage(new MemoryStorageOptions
                {
                    JobExpirationCheckInterval = TimeSpan.FromMinutes(30)
                });

                config.Server = new BackgroundJobServer(new BackgroundJobServerOptions
                {
                    WorkerCount = 1
                });
                config.GlobalConfiguration.UseConsole();
            });
        }
    }
}