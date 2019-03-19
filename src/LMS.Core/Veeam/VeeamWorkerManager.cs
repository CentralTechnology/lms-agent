namespace LMS.Core.Veeam
{
    using System;
    using System.Threading.Tasks;
    using Core.Managers;
    using Extensions;
    using Factory;
    using global::Hangfire.Console;
    using global::Hangfire.Server;
    using Hangfire;
    using Managers;
    using Serilog;
    using Services;
    using Services.Authentication;

    public class VeeamWorkerManager : WorkerManagerBase, IVeeamWorkerManager
    {
        private readonly ILogger _logger = Log.ForContext<VeeamWorkerManager>();
        private readonly IVeeamManager _veeamManager;

        public VeeamWorkerManager(
            IPortalService portalService,
            IVeeamManager veeamManager)
            : base(portalService)
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
                    _logger.Error("Veeam server not online.");
                    return;
                }

                performContext?.WriteLine("Collecting information...this could take some time.");
                var veeamVersion = _veeamManager.GetInstalledVeeamVersion();

                performContext?.WriteLine($"Veeam Backup & Replication {veeamVersion} detected!");

                var factory = new VeeamCreatorFactory(veeamVersion);
                var creator = factory.GetCreator();
                var result = creator.Create();
                if (result.IsFailed)
                {
                    // log
                    foreach (var error in result.Errors)
                    {
                        performContext?.WriteErrorLine(error.Message);
                    }

                    _logger.Error("{Errors}", result.Errors);

                    throw new Exception(result.Errors.ToString());
                }

                performContext?.WriteLine("done");

                performContext?.WriteLine("Validating the payload...");
                var remoteVeeam = PortalService.GetVeeamServerById(PortalAuthenticationService.Instance.GetDevice());
                if (remoteVeeam.Count != 1)
                {
                    result.Value.Validate();
                    performContext?.WriteSuccessLine("Payload is valid!");

                    _logger.Information("Payload: {@Payload}", result.Value);

                    await PortalService.AddVeeamServerAsync(result.Value);

                    performContext?.WriteSuccessLine("Successfully checked in.");

                    return;
                }

                remoteVeeam[0].UpdateValues(result.Value);
                remoteVeeam[0].Validate();

                performContext?.WriteLine("Payload is valid!");

                _logger.Information("Payload: {@Payload}", result.Value);

                await PortalService.UpdateVeeamServerAsync(remoteVeeam[0]);

                performContext?.WriteSuccessLine("Successfully checked in.");
            });
        }
    }
}