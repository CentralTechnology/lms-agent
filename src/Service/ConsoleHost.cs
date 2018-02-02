namespace LMS.Service
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Security;
    using Abp;
    using Abp.Castle.Logging.Log4Net;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Logging;
    using Castle.Facilities.Logging;
    using Common.Helpers;
    using Configuration;
    using LMS.Startup;
    using Microsoft.OData.Client;
    using Users;
    using Veeam;

    public static class ConsoleHost
    {
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

                    if (opts.DeviceId != default(Guid))
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, opts.DeviceId.ToString());
                        Console.WriteLine($"Device ID: {opts.DeviceId}");
                    }

                    if (opts.PdcOverride.HasValue)
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.PrimaryDomainControllerOverride, opts.PdcOverride.Value.ToString());
                        Console.WriteLine($"PDC Override: {opts.PdcOverride.Value}");
                    }

                    if (opts.UsersOverride.HasValue)
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.UsersOverride, opts.UsersOverride.Value.ToString());
                        Console.WriteLine($"Users Override: {opts.UsersOverride.Value}");
                    }

                    if (opts.VeeamOverride.HasValue)
                    {
                        settingsManager.Object.ChangeSettingForApplication(AppSettingNames.VeeamOverride, opts.VeeamOverride.Value.ToString());
                        Console.WriteLine($"Veeam Override: {opts.VeeamOverride.Value}");
                    }
                }
            }
        }
        public static void Run(RunOptions opts)
        {
            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
            {
                bootstrapper.Initialize();


                if (opts.Verbose)
                {
                    Log4NetHelper.EnableDebug();
                }

                LogHelper.Logger.Info($"Version: {AppVersionHelper.Version}  Release: {AppVersionHelper.ReleaseDate}");

                if (!opts.SkipStartup)
                {
                    using (IDisposableDependencyObjectWrapper<StartupManager> startupManager = bootstrapper.IocManager.ResolveAsDisposable<StartupManager>())
                    {
                        bool started = startupManager.Object.Init(null);
                        if (!started)
                        {
                            return;
                        }
                    }
                }

                try
                {
                    if (opts.Monitor == Monitor.Users)
                    {
                        using (IDisposableDependencyObjectWrapper<UserWorkerManager> userWorkerManager = bootstrapper.IocManager.ResolveAsDisposable<UserWorkerManager>())
                        {
                            userWorkerManager.Object.Start(null);
                            return;
                        }
                    }

                    if (opts.Monitor == Monitor.Veeam)
                    {
                        using (IDisposableDependencyObjectWrapper<VeeamWorkerManager> veeamWorkerManager = bootstrapper.IocManager.ResolveAsDisposable<VeeamWorkerManager>())
                        {
                            veeamWorkerManager.Object.Start(null);
                        }
                    }

                    Console.WriteLine("Press [Enter] to continue.");
                    Console.ReadLine();
                }
                catch (Exception ex) when (
                    ex is DataServiceClientException
                    || ex is SqlException
                    || ex is HttpRequestException
                    || ex is SocketException
                    || ex is WebException
                    || ex is SecurityException
                    || ex is IOException)
                {
                    LogHelper.LogException(ex);
                    LogHelper.Logger.Debug(ex.Message, ex);
                    LogHelper.Logger.Error("************ Failed ************");
                }
                catch (Exception ex)
                {
                    // sentry
                    LogHelper.LogException(ex);
                    LogHelper.Logger.Debug(ex.Message, ex);
                    LogHelper.Logger.Error("************ Failed ************");
                }

                Console.WriteLine("Press [Enter] to continue.");
                Console.ReadLine();
            }
        }
    }
}