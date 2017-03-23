namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using Abp;
    using Abp.Dependency;
    using Abp.Extensions;
    using Castle.Core.Logging;
    using Castle.Facilities.Logging;
    using Core;
    using Core.Administration;
    using Core.Common.Enum;
    using Core.Common.Extensions;
    using Menu;

    class Runner
    {
        public const string ServiceName = "LMS";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<LicenseMonitoringModule>())
            {
                bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog("NLog.config"));
                bootstrapper.Initialize();

                using (IDisposableDependencyObjectWrapper<ILogger> logger = bootstrapper.IocManager.ResolveAsDisposable<ILogger>())
                {
                    if (Environment.UserInteractive)
                    {
                        Console.WindowWidth = Console.LargestWindowWidth / 2;
                        Console.WindowHeight = Console.LargestWindowHeight / 3;

                        try
                        {
                            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingsManager = bootstrapper.IocManager.ResolveAsDisposable<ISettingsManager>())
                            {
                                settingsManager.Object.Validate();
                            }
                            Console.Clear();
                            new ClientProgram().Run();
                        }
                        catch (Exception ex)
                        {
                            logger.Object.Error(ex.Message);
                            logger.Object.DebugFormat("Exception: ", ex);

                            Console.ReadKey(true);
                            Environment.Exit(ex.HResult);
                        }
                    }
                    else
                    {
                        using (MonitoringService service = new MonitoringService())
                        {
                            try
                            {
                                ServiceBase.Run(service);
                            }
                            catch (Exception ex)
                            {
                                logger.Object.DebugFormat("Exception: ", ex);
                                service.Stop();
                            }
                        }
                    }
                }
            }
        }

        public class MonitoringService : ServiceBase, ITransientDependency
        {
            public MonitoringService()
            {
                Logger = NullLogger.Instance;

                ServiceName = Runner.ServiceName;
                CanStop = true;
                CanPauseAndContinue = false;
                AutoLog = true;
            }

            public ILogger Logger { get; set; }

            protected override void OnStart(string[] args)
            {
                Logger.Info("Starting service");

                System.Timers.Timer timer = new System.Timers.Timer {Interval = 90000};
                timer.Elapsed += OnTimer;
                timer.Start();
            }

            protected override void OnStop()
            {
                Logger.Info("Shutting down service");
            }

            public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
            {
                using (IDisposableDependencyObjectWrapper<Orchestrator> orchestrator = IocManager.Instance.ResolveAsDisposable<Orchestrator>())
                {
                    using (IDisposableDependencyObjectWrapper<ISettingsManager> settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
                    {
                        settingsManager.Object.Validate();

                        List<Monitor> monitors = settingsManager.Object.Read().Monitors.GetFlags().As<List<Monitor>>();

                        foreach (Monitor monitor in monitors)
                        {
                            Logger.Info($"running monitor: {monitor}");
                            orchestrator.Object.Run(monitor);
                        }
                    }
                }
            }
        }
    }
}