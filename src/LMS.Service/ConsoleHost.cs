namespace LMS
{
    using System;
    using Abp;
    using Abp.Configuration;
    using Abp.Dependency;
    using Core;
    using Core.Configuration;
    using Core.Helpers;
    using Core.StartUp;
    using Core.Users;
    using Core.Veeam;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

    public static class ConsoleHost
    {
        private static readonly ILogger Logger = Log.Logger;

        public static void Run(RunOptions opts)
        {
            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
            {
                bootstrapper.Initialize();

                if (opts.Verbose)
                {
                    Program.CurrentLogLevel.MinimumLevel = LogEventLevel.Debug;
                    Log.Debug("Verbose logging enabled.");
                }

                Logger.Information($"Version: {AppVersionHelper.Version}  Release: {AppVersionHelper.ReleaseDate}");

                using (var iocResolver = bootstrapper.IocManager.ResolveAsDisposable<IIocResolver>())
                {
                    using (var scope = iocResolver.Object.CreateScope())
                    {
                        if (!opts.SkipStartup)
                        {
                            var startupManager = scope.Resolve<IStartupManager>();
                            bool started = startupManager.Init();
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
                                var userWorkerTask = userWorkerManager.StartAsync(null);
                                userWorkerTask.Wait();

                                return;
                            }

                            if (opts.Monitor == Monitor.Veeam)
                            {
                                var veeamWorkerManager = scope.Resolve<VeeamWorkerManager>();
                                var veeamWorkerTask = veeamWorkerManager.StartAsync(null);
                                veeamWorkerTask.Wait();

                                return;
                            }

                            Log.CloseAndFlush();
                            Console.WriteLine("Press [Enter] to continue.");
                            Console.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug(ex, ex.Message);
                            Logger.Error(ex.Message);
                            Logger.Error("************ Failed ************");
                        }
                    }
                }
            }
        }

        public static void Update(UpdateOptions opts)
        {
            using (var bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
            {
                bootstrapper.Initialize();

                using (var settingsManager = bootstrapper.IocManager.ResolveAsDisposable<ISettingManager>())
                {
                    if (opts.AccountId.HasValue && opts.AccountId != default(int))
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