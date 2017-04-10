namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Abp.Threading;
    using Common.Enum;
    using Models;
    using ShellProgressBar;
    using Users;

    public class OrchestratorManager : DomainService, IOrchestratorManager
    {
        private readonly IUserOrchestrator _userOrchestrator;

        public OrchestratorManager(IUserOrchestrator userOrchestrator)
        {
            _userOrchestrator = userOrchestrator;
        }

        public void Run(Monitor monitor)
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

        public async Task UserMonitor()
        {
            int initialProgress = 1;

            using (ProgressBar pbar = Environment.UserInteractive ? new ProgressBar(initialProgress, "overall progress", ConsoleColor.DarkGray) : null)
            {
                try
                {
                    var upload = await _userOrchestrator.ProcessUpload(pbar);

                    if (upload == null || CallInStatus.CalledIn.HasFlag(upload.Status))
                    {
                        return;
                    }

                    pbar?.UpdateMaxTicks(initialProgress + 4);

                    List<LicenseUser> users = await _userOrchestrator.ProcessUsers(upload.Id, pbar);

                    await _userOrchestrator.ProcessGroups(users, pbar);

                    await _userOrchestrator.ProcessUserGroups(users, pbar);

                    await _userOrchestrator.CallIn(upload.Id, pbar);
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