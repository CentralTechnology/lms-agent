namespace Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Abp.Threading;
    using Common.Enum;
    using Factory;
    using Models;
    using NLog;
    using Users;

    public class OrchestratorManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            ManagedSupport upload = await OrchestratorFactory.UserOrchestrator().ProcessUpload();

            if (upload == null || CallInStatus.CalledIn.HasFlag(upload.Status))
            {
                return;
            }

            List<LicenseUser> users = await OrchestratorFactory.UserOrchestrator().ProcessUsers(upload.Id);

            await OrchestratorFactory.UserOrchestrator().ProcessGroups(users);

            await OrchestratorFactory.UserOrchestrator().ProcessUserGroups(users);

            await OrchestratorFactory.UserOrchestrator().CallIn(upload.Id);

            Logger.Info("Finished.");
        }
    }
}