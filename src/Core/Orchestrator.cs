namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.AutoMapper;
    using Abp.Dependency;
    using Abp.Threading;
    using Abp.Timing;
    using Castle.Core.Logging;
    using Common.Client;
    using Common.Extensions;
    using Models;
    using Settings;
    using ShellProgressBar;
    using Users;

    public class Orchestrator : LicenseMonitoringBase, ISingletonDependency
    {
        public void Run(Monitor monitor)
        {
            using (var settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
            {
                switch (monitor)
                {
                    case Monitor.Users:
                        
                        AsyncHelper.RunSync(UserMonitor);
                        break;
                    default:
                        Logger.Info("No licenses are set to be monitored");
                        break;
                }
            }
        }

        private async Task UserMonitor()
        {
            ConsoleExtensions.WriteLineBottom("Logs \n", LoggerLevel.Info);
            using (var userOrchestrator = IocManager.Instance.ResolveAsDisposable<IUserOrchestrator>())
            {
                int uploadId = await userOrchestrator.Object.ProcessUpload();
                if (uploadId == 0)
                {
                    return;
                }

                using (var pbar = new ProgressBar(4, "overall progress", ConsoleColor.DarkGray))
                {
                    var users = await userOrchestrator.Object.ProcessUsers(uploadId, pbar);

                    await userOrchestrator.Object.ProcessGroups(users, pbar);

                    await userOrchestrator.Object.ProcessUserGroups(users, pbar);

                    await userOrchestrator.Object.CallIn(uploadId);
                }
            }
        }
    }
}