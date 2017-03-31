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

            _timer = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds) {AutoReset = true};
            _timer.Elapsed += TimerElapsed;
        }

        public ILogger Logger { get; set; }

        public bool Start()
        {
            try
            {
                _settingsManager.Validate();

                if (Environment.UserInteractive)
                {
                    Console.WindowWidth = Console.LargestWindowWidth / 2;
                    Console.WindowHeight = Console.LargestWindowHeight / 3;
                    Console.Clear();
                    new ClientProgram(new Guid()).Run();
                }
                else
                {
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex.ToString);
            }

            return true;
        }

        public bool Stop()
        {
            _timer.Stop();
            _timer.Dispose();
            return true;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs args)
        {
           var monitors = _settingsManager.Read().Monitors.GetFlags().As<List<Monitor>>();

            foreach (Monitor monitor in monitors)
            {
                Logger.Info($"Started action: {monitor} \t {DateTime.UtcNow}");

                _orchestratorManager.Run(monitor);

                Logger.Info($"Completed action: {monitor} \t {DateTime.UtcNow}");
            }
        }
    }
}