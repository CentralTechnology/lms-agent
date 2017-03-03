namespace Service
{
    using System;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Threading;
    using Abp;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Castle.Facilities.Logging;
    using Core;
    using Core.Settings;
    using Menu;

    class Runner
    {
        public const string ServiceName = "LMS";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var bootstrapper = AbpBootstrapper.Create<LicenseMonitoringModule>())
            {
                bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog("NLog.config"));
                bootstrapper.Initialize();

                using (var logger = bootstrapper.IocManager.ResolveAsDisposable<ILogger>())
                {
                    if (Environment.UserInteractive)
                    {
                        Console.WindowWidth = Console.LargestWindowWidth / 2;
                        Console.WindowHeight = Console.LargestWindowHeight / 3;

                        try
                        {
                            using (var settingsManager = bootstrapper.IocManager.ResolveAsDisposable<ISettingManager>())
                            {
                                settingsManager.Object.LoadSettings();
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
                        using (var service = new MonitoringService())
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
                using (var settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
                {
                    settingsManager.Object.LoadSettings();
                }

                System.Timers.Timer timer = new System.Timers.Timer {Interval = 90000 };
                timer.Elapsed += OnTimer;
                timer.Start();
            }

            protected override void OnStop()
            {
                Logger.Info("Shutting down service");
            }

            public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
            {
                using (var orchestrator = IocManager.Instance.ResolveAsDisposable<Orchestrator>())
                {
                    using (var settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
                    {                       
                        var monitors = settingsManager.Object.GetMonitors();

                        foreach (var monitor in monitors)
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