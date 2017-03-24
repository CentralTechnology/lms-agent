namespace Core
{
    using System;
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Abp.Domain.Services;
    using Abp.Threading;
    using Administration;
    using Common.Enum;
    using ShellProgressBar;
    using Users;

    public class OrchestratorManager : DomainService, IOrchestratorManager
    {
        public void Run(Monitor monitor)
        {
            using (var settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                switch (monitor)
                {
                    case Monitor.None:
                        Logger.Info("No licenses are set to be monitored");
                        break;
                    case Monitor.Users:

                        AsyncHelper.RunSync(UserMonitor);
                        break;
                    default:
                        Logger.Info("No licenses are set to be monitored");
                        break;
                }
            }
        }

        public async Task UserMonitor()
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