namespace Service
{
    using System;
    using System.ServiceProcess;
    using Abp;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Castle.Facilities.Logging;
    using Core;
    using Core.Settings;
    using LicenseMonitoringSystem;
    using Menu;

    class Runner
    {
        public const string ServiceName = "LMS";
        private static readonly AbpBootstrapper Bootstrapper = AbpBootstrapper.Create<LicenseMonitoringModule>();

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog("NLog.config"));
            Bootstrapper.Initialize();

            var logger = Bootstrapper.IocManager.Resolve<ILogger>();

            if (Environment.UserInteractive)
            {
                try
                {
                    new ClientProgram().Run();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.DebugFormat("Exception: ", ex);

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
                        logger.DebugFormat("Exception: ", ex);
                        service.Stop();
                    }
                }
            }
        }

        public class MonitoringService : ServiceBase
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
                Logger.Debug("Starting service");

                System.Timers.Timer timer = new System.Timers.Timer {Interval = 60000};
                timer.Elapsed += OnTimer;
                timer.Start();
            }

            protected override void OnStop()
            {
                Logger.Debug("Shutting down service");
                Bootstrapper.Dispose();
            }

            public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
            {
                using (var orchestrator = Bootstrapper.IocManager.ResolveAsDisposable<Orchestrator>())
                {
                    using (var settingsManager = Bootstrapper.IocManager.ResolveAsDisposable<ISettingManager>())
                    {
                        settingsManager.Object.FirstRun();

                        foreach (var monitor in settingsManager.Object.GetMonitors())
                        {
                            orchestrator.Object.Run(monitor);
                        }
                    }
                }
            }
        }
    }
}