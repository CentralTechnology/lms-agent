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
            var upload = await _userOrchestrator.ProcessUpload();

            if (upload == null || CallInStatus.CalledIn.HasFlag(upload.Status))
            {
                return;
            }

            List<LicenseUser> users = await _userOrchestrator.ProcessUsers(upload.Id);

            await _userOrchestrator.ProcessGroups(users);

            await _userOrchestrator.ProcessUserGroups(users);

            await _userOrchestrator.CallIn(upload.Id);
        }
    }
}