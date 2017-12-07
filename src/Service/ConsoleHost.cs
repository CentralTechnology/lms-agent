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
    using Abp.Dependency;
    using Abp.Logging;
    using Castle.Facilities.Logging;
    using LMS.Common.Helpers;
    using LMS.Startup;
    using Microsoft.OData.Client;
    using Users;
    using Veeam;

    public class ConsoleHost
    {
        public static void Run(RunOptions opts)
        {
            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
            {
                bootstrapper.IocManager
                    .IocContainer
                    .AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

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
                            veeamWorkerManager.Object.Start();
                        }
                    }
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