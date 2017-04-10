namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Timers;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core;
    using Core.Administration;
    using Core.Common.Enum;
    using Menu;
    using Core.Common.Extensions;
    using Abp.Extensions;
    using System.Linq;
    using Abp.Timing;

    public class LicenseMonitoringSystemService : ITransientDependency
    {
        private readonly IOrchestratorManager _orchestratorManager;
        private readonly ISettingsManager _settingsManager;
        private readonly Timer _timer;

        public LicenseMonitoringSystemService(ISettingsManager settingManager, IOrchestratorManager orchestratorManager)
        {
            _settingsManager = settingManager;
            _orchestratorManager = orchestratorManager;

            Logger = NullLogger.Instance;

            _timer = new Timer();
            _timer.Elapsed += TimerElapsed;
            _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            _timer.AutoReset = false;
        }

        public ILogger Logger { get; set; }

        public bool Start()
        {
            Logger.Info("Service started");


            if (Environment.UserInteractive)
            {
                try
                {
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

        private void TimerElapsed(object sender, ElapsedEventArgs args)
        {
            try
            {
                Logger.Debug("Timer elapsed");

                var monitors = _settingsManager.Read().Monitors.GetFlags().ToList();

                Logger.Info($"Found {monitors.Count} monitors");

                foreach (Monitor monitor in monitors)
                {
                    Logger.Info($"Started action: {monitor} \t {Clock.Now}");

                    _orchestratorManager.Run(monitor);

                    Logger.Info($"Completed action: {monitor} \t {Clock.Now}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}