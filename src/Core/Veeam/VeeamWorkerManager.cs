namespace LMS.Core.Veeam
{
    using System;
    using System.Threading.Tasks;
    using Common.Extensions;
    using global::Hangfire.Server;
    using Hangfire;
    using LMS.Common.Managers;
    using LMS.Veeam;
    using LMS.Veeam.Managers;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamWorkerManager : WorkerManagerBase, IVeeamWorkerManager
    {
        private readonly IVeeamManager _veeamManager;

        public VeeamWorkerManager(IVeeamManager veeamManager)
        {
            _veeamManager = veeamManager;
        }

        [Mutex("VeeamWorkerManager")]
        public override void Start(PerformContext performContext) => throw new NotImplementedException();

        [Mutex("VeeamWorkerManager")]
        public override async Task StartAsync(PerformContext performContext)
        {
            await ExecuteAsync(performContext, async () =>
            {
                if (!_veeamManager.IsOnline())
                {
                    performContext?.WriteErrorLine("The Veeam server does not appear to be online. Please make sure all Veeam services are started before retrying this operation again.");
                    Logger.Error("Veeam server not online.");
                    return;
                }

                Logger.Info(performContext, "Collecting information...this could take some time.");

                Veeam payload = _veeamManager.GetLicensingInformation(performContext);

                Logger.Info(performContext, "done");
                Logger.Info(performContext, "Validating the payload...");

                payload.Validate();

                Logger.Info(performContext, "Payload is valid!");
                Logger.Info($"Payload details: {JsonConvert.SerializeObject(payload, Formatting.None)}");

                await PortalService.UpdateVeeamServerAsync(payload);
            });
        }
    }
}