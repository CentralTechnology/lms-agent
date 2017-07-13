namespace Service
{
    using System;
    using System.Timers;
    using Core;
    using Core.Administration;
    using Core.Common.Enum;
    using Core.Factory;
    using Menu;
    using NLog;

    public class LicenseMonitoringSystemService
    {
        private static Timer _timer;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public const string ServiceName = "LicenseMonitoringSystem";
        public const string ServiceDisplayName = "License Monitoring System";
        public const string ServiceDescription = "A tool used to monitor various licenses.";

        public LicenseMonitoringSystemService()
        {
            _timer = new Timer();
            _timer.Elapsed += UserMonitor;
            _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        }
       

        public bool Start()
        {
            Logger.Info("Service started");

            if (Environment.UserInteractive)
            {
                try
                {
                    SettingFactory.SettingsManager().Validate();

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
            SettingFactory.SettingsManager().Validate();

            try
            {
                var monitors = SettingFactory.SettingsManager().Read().Monitors;

                if (!Monitor.Users.HasFlag(monitors))
                {
                    return;
                }

                Logger.Info("Users monitor begin.");

                OrchestratorFactory.Orchestrator().Run(Monitor.Users);

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