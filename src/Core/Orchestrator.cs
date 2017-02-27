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
    using Common.Client;
    using Common.Extensions;
    using Models;
    using Settings;
    using Users;

    public class Orchestrator : LicenseMonitoringBase, ISingletonDependency
    {
        public Orchestrator()
        {
        }

        public void Run(Monitor monitor)
        {
            switch (monitor)
            {
                case Monitor.Users:
                    AsyncHelper.RunSync(() => UserMonitor());
                    break;
                default:
                    Logger.Info("No licenses are set to be monitored");
                    break;
            }
        }

        private async Task UserMonitor()
        {
            using (var userOrchestrator = IocManager.Instance.ResolveAsDisposable<IUserOrchestrator>())
            {
                int uploadId = await userOrchestrator.Object.ProcessUpload();
                if (uploadId == 0)
                {
                    return;
                }

                Console.WriteLine("########################################");
                Console.WriteLine("#              USERS BEGIN             #");
                Console.WriteLine("########################################");
                Console.WriteLine(Environment.NewLine);

                var users = await userOrchestrator.Object.ProcessUsers(uploadId);

                Console.WriteLine("########################################");
                Console.WriteLine("#               USERS END              #");
                Console.WriteLine("########################################");
                Console.WriteLine(Environment.NewLine);

                Console.WriteLine("########################################");
                Console.WriteLine("#             GROUPS BEGIN             #");
                Console.WriteLine("########################################");
                Console.WriteLine(Environment.NewLine);

                await userOrchestrator.Object.ProcessGroups(users);

                Console.WriteLine("########################################");
                Console.WriteLine("#              GROUPS END              #");
                Console.WriteLine("########################################");
                Console.WriteLine(Environment.NewLine);

                await userOrchestrator.Object.CallIn(uploadId);
            }
        }
    }
}