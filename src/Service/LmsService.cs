namespace Service
{
    using System;
    using Core.Administration;
    using Core.Common.Extensions;
    using Core.Common.Helpers;
    using Menu;
    using ServiceTimer;
    using Workers;

    public class LmsService : TimerServiceBase
    {
        private static readonly StartupManager StartupManager = new StartupManager();
        private static readonly SettingManager SettingManager = new SettingManager();
        public override bool Start()
        {
            DefaultLog();

            Log.Info($"Version: {AppVersionHelper.Version}  Release: {AppVersionHelper.ReleaseDate}");
            
            StartupManager.Init();

            if (Environment.UserInteractive)
            {
                Console.Clear();
                new ClientProgram(Guid.NewGuid()).Run();
            }
            else
            {
                if (SettingManager.GetSettingValue<bool>(SettingNames.MonitorVeeam))
                {
                    var veeamMonitorWorker = new VeeamMonitorWorker();
                    RegisterWorker(veeamMonitorWorker);
                }

                if (SettingManager.GetSettingValue<bool>(SettingNames.MonitorUsers))
                {
                    var userMonitorWorker = new UserMonitorWorker();
                    RegisterWorker(userMonitorWorker);
                }
            }

            return true;
        }
    }
}