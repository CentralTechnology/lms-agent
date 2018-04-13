namespace LMS.Core.Veeam
{
    using System.Threading.Tasks;
    using Core.Managers;
    using Extensions;
    using global::Hangfire.Server;
    using Hangfire;
    using Managers;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using Services;
    using Services.Authentication;

    public class VeeamWorkerManager : WorkerManagerBase, IVeeamWorkerManager
    {
        private readonly IVeeamManager _veeamManager;

        public VeeamWorkerManager(
            IPortalService portalService,
            IPortalAuthenticationService authService,
            IVeeamManager veeamManager)
            : base(portalService, authService)
        {
            _veeamManager = veeamManager;
        }

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

                var prettyPayload = new
                {
                    Hostname = payload.Hostname,
                    Id = payload.Id,
                    Agent = payload.ClientVersion,
                    Tenant = payload.TenantId,
                    Edition = payload.Edition.ToString(),
                    LicenseType = payload.LicenseType.ToString(),
                    HyperV = payload.HyperV,
                    VMWare = payload.vSphere,
                    ExpirationDate = payload.ExpirationDate.ToString("o"),
                    Program = payload.ProgramVersion,
                    SupportId = payload.SupportId
                };

                Logger.Info($"Payload details: {JsonConvert.SerializeObject(prettyPayload, Formatting.Indented)}");

                await PortalService.UpdateVeeamServerAsync(payload);

                Logger.Info(performContext,"Successfully checked in.");
            });
        }
    }
}