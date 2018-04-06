namespace LMS.Service
{
    using System;
    using Abp;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Logging;
    using Abp.Threading;
    using Castle.Core.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Services.Logging.SerilogIntegration;
    using Common.Helpers;
    using Configuration;
    using Core.Veeam;
    using LMS.Startup;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Users;

    public static class ConsoleHost
    {
        public static void Run(RunOptions opts)
        {
            LMSCoreModule.CurrentLogLevel = new LoggingLevelSwitch();

            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
            {
                bootstrapper.Initialize();

                bootstrapper.IocManager.IocContainer.Register(
                    Component.For<LoggerConfiguration>()
                        .UsingFactoryMethod(
                            () => new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug()
                             //   .MinimumLevel.ControlledBy(LMSCoreModule.CurrentLogLevel)
                                
                               // .WriteTo.File("logs/log.txt", fileSizeLimitBytes: 5242880, rollOnFileSizeLimit: true, retainedFileCountLimit: 10)
                            
                        ).LifestyleSingleton(),
                    Component.For<ILoggerFactory, SerilogFactory>()
                        .ImplementedBy<SerilogFactory>()
                        .LifestyleSingleton(),
                    Component.For<Castle.Core.Logging.ILogger>()
                        .UsingFactoryMethod(
                            (kernel, componentModel, creationContext) => kernel.Resolve<ILoggerFactory>().Create(creationContext.Handler.ComponentModel.Name)
                        ).LifestyleTransient()
                );

                if (opts.Verbose)
                {
                    LMSCoreModule.CurrentLogLevel.MinimumLevel = LogEventLevel.Debug;
                    LogHelper.Logger.Debug("Verbose logging enabled.");
                }

                LogHelper.Logger.Info($"Version: {AppVersionHelper.Version}  Release: {AppVersionHelper.ReleaseDate}");

                using (var iocResolver = bootstrapper.IocManager.ResolveAsDisposable<IIocResolver>())
                {
                    using (var scope = iocResolver.Object.CreateScope())
                    {
                        if (!opts.SkipStartup)
                        {
                            var startupManager = scope.Resolve<IStartupManager>();
                            bool started = startupManager.Init(null);
                            if (!started)
                            {
                                return;
                            }
                        }

                        try
                        {
                            if (opts.Monitor == Monitor.Users)
                            {
                                var userWorkerManager = scope.Resolve<UserWorkerManager>();
                                AsyncHelper.RunSync(() => userWorkerManager.StartAsync(null));
                                return;
                            }

                            if (opts.Monitor == Monitor.Veeam)
                            {
                                var veeamWorkerManager = scope.Resolve<VeeamWorkerManager>();
                                AsyncHelper.RunSync(() => veeamWorkerManager.StartAsync(null));
                                return;
                            }

                            Console.WriteLine("Press [Enter] to continue.");
                            Console.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogException(ex);
                            LogHelper.Logger.Debug(ex.Message, ex);
                            LogHelper.Logger.Error("************ Failed ************");
                        }
                    }
                }

                Console.WriteLine("Press [Enter] to continue.");
                Console.ReadLine();
            }
        }

        public static void Update(UpdateOptions opts)
        {
            using (var bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
            {
                bootstrapper.Initialize();

                using (var settingsManager = bootstrapper.IocManager.ResolveAsDisposable<ISettingManager>())
                {
                    if (opts.AccountId != default(int))
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, opts.AccountId.ToString());
                        Console.WriteLine($"Account ID: {opts.AccountId}");
                    }

                    if (!string.IsNullOrEmpty(opts.DeviceId))
                    {
                        if (Guid.TryParse(opts.DeviceId, out Guid deviceId))
                        {
                            settingsManager.Object.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, deviceId.ToString());
                            Console.WriteLine($"Device ID: {deviceId}");
                        }
                    }

                    if (opts.PdcOverride.HasValue)
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.PrimaryDomainControllerOverride, opts.PdcOverride.Value.ToString());
                        Console.WriteLine($"PDC Override: {opts.PdcOverride.Value}");
                    }

                    if (opts.UsersOverride.HasValue)
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.UserMonitorEnabled, opts.UsersOverride.Value.ToString());
                        Console.WriteLine($"Users Override: {opts.UsersOverride.Value}");
                    }

                    if (opts.VeeamOverride.HasValue)
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.VeeamMonitorEnabled, opts.VeeamOverride.Value.ToString());
                        Console.WriteLine($"Veeam Override: {opts.VeeamOverride.Value}");
                    }
                }
            }
        }
    }
}