namespace LMS
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Abp.Hangfire;
    using Abp.Hangfire.Configuration;
    using Abp.Modules;
    using global::Hangfire;
    using global::Hangfire.Console;
    using global::Hangfire.MemoryStorage;

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