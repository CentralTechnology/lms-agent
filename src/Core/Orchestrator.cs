namespace Core
{
    using System;
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Abp.Threading;
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
            int initialProgress = 1;
            using (var userOrchestrator = IocManager.Instance.ResolveAsDisposable<IUserOrchestrator>())
            {
                using (var pbar = Environment.UserInteractive ? new ProgressBar(initialProgress, "overall progress", ConsoleColor.DarkGray) : null)
                {
                    try
                    {
                        int uploadId = await userOrchestrator.Object.ProcessUpload(pbar);

                        if (uploadId == 0)
                        {
                            return;
                        }

                        pbar?.UpdateMaxTicks(initialProgress + 4);

                        var users = await userOrchestrator.Object.ProcessUsers(uploadId, pbar);

                        await userOrchestrator.Object.ProcessGroups(users, pbar);

                        await userOrchestrator.Object.ProcessUserGroups(users, pbar);

                        await userOrchestrator.Object.CallIn(uploadId, pbar);
                    }
                    catch (Exception ex)
                    {
                        pbar?.UpdateMessage(ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}