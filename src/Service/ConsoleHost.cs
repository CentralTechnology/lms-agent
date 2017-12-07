namespace LMS
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
    using Common.Helpers;
    using Microsoft.OData.Client;
    using Startup;
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
                        bool started = startupManager.Object.Init();
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
                        using (IDisposableDependencyObjectWrapper<IUserWorkerManager> userWorkerManager = bootstrapper.IocManager.ResolveAsDisposable<IUserWorkerManager>())
                        {
                            userWorkerManager.Object.Start();
                            return;
                        }
                    }

                    if (opts.Monitor == Monitor.Veeam)
                    {
                        using (IDisposableDependencyObjectWrapper<IVeeamWorkerManager> veeamWorkerManager = bootstrapper.IocManager.ResolveAsDisposable<IVeeamWorkerManager>())
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
            }
        }
    }
}