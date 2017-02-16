namespace LicenseMonitoringSystem
{
    using System;
    using System.ServiceProcess;
    using System.Threading;
    using Abp;
    using Abp.Dependency;
    using Castle.Facilities.Logging;
    using Core;
    using Core.Common.Client;
    using Core.Settings;
    using Fclp;

    static class Program
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

            if (Environment.UserInteractive)
            {
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
            else
            {
                using (var service = new MonitoringService())
                {
                    ServiceBase.Run(service);
                }
            }
        }

        private static void Start(object obj)
        {
            Console.WriteLine("Starting...");

            var args = (string[]) obj;

            var parser = new FluentCommandLineParser<ApplicationArguments>();

            parser.Setup(arg => arg.Debug)
                .As('d', "debug")
                .SetDefault(false);

            var result = parser.Parse(args);

            if (!result.HasErrors)
            {
                using (var orchestrator = Bootstrapper.IocManager.ResolveAsDisposable<Orchestrator>())
                {
                    using (var settingsManager = Bootstrapper.IocManager.ResolveAsDisposable<SettingManager>())
                    {
                        // set log level
                        settingsManager.Object.SetDebug(parser.Object.Debug);

                        // check settings exist
                        var settings = settingsManager.Object.Exists();
                        if (!settings)
                        {                                      
                            // get the account id
                            long accountId;
                            using (var apiClient = Bootstrapper.IocManager.ResolveAsDisposable<PortalClient>())
                            {
                                accountId = apiClient.Object.GetAccountId(settingsManager.Object.GetDeviceId());
                            }

                            // create
                            settingsManager.Object.Create(accountId);                            
                        }

                        foreach (var monitor in settingsManager.Object.GetMonitors())
                        {
                            orchestrator.Object.Run(monitor);
                        }
                    }
                }
            }
        }

        private static void Stop()
        {
            Console.WriteLine("Stopping...");
            Bootstrapper.Dispose();
        }

        public class MonitoringService : ServiceBase
        {
            private Timer _stateTimer;
            private TimerCallback _timerDelegate;

            public MonitoringService()
            {
                ServiceName = Program.ServiceName;
                CanStop = true;
                CanPauseAndContinue = false;
                AutoLog = true;
            }

            protected override void OnStart(string[] args)
            {
                _timerDelegate = Start;
                _stateTimer = new Timer(_timerDelegate, args, 30000, 30000);
            }

            protected override void OnStop()
            {
                Stop();
                _stateTimer.Dispose();
            }
        }
    }
}