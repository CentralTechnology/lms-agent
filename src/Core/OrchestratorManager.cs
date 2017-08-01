namespace Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Factory;
    using Models;

    public class OrchestratorManager
    {
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
        }

        public async Task VeeamMonitor()
        {
        }
    }
}