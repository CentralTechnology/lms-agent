namespace Service
{
    using System;
    using System.Timers;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core;
    using Core.Administration;
    using Core.Common.Enum;
    using Menu;

    public class LicenseMonitoringSystemService : ISingletonDependency
    {
        private static Timer _timer;
        private readonly IOrchestratorManager _orchestratorManager;
        private readonly ISettingsManager _settingsManager;

        public const string ServiceName = "LicenseMonitoringSystem";
        public const string ServiceDisplayName = "License Monitoring System";
        public const string ServiceDescription = "A tool used to monitor various licenses.";

        public LicenseMonitoringSystemService(ISettingsManager settingManager, IOrchestratorManager orchestratorManager)
        {
            _settingsManager = settingManager;
            _orchestratorManager = orchestratorManager;

            Logger = NullLogger.Instance;

            _timer = new Timer();
            _timer.Elapsed += UserMonitor;
            _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        }

        public ILogger Logger { get; set; }

        public bool Start()
        {
            Logger.Info("Service started");

            if (Environment.UserInteractive)
            {
                try
                {
                    _settingsManager.Validate();

                    Console.WindowWidth = Console.LargestWindowWidth / 2;
                    Console.WindowHeight = Console.LargestWindowHeight / 3;
                    Console.Clear();
                    new ClientProgram(new Guid()).Run();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    Logger.Debug(ex.ToString);
                }
            }
            else
            {
                _timer.Start();
            }

            return true;
        }

        public bool Stop()
        {
            _timer.Stop();
            _timer.Dispose();
            Logger.Info("Service stopped");

            return true;
        }

        private void UserMonitor(object sender, ElapsedEventArgs args)
        {
            _settingsManager.Validate();

            try
            {
                var monitors = _settingsManager.Read().Monitors;

                if (!Monitor.Users.HasFlag(monitors))
                {
                    return;
                }

                Logger.Info("Users monitor begin.");

                _orchestratorManager.Run(Monitor.Users);

                Logger.Info("Users monitor end.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex.ToString());
            }
        }
    }
}