namespace LMS.Core.Veeam
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Abp.Configuration;
    using Configuration;
    using Core.Managers;
    using Extensions;
    using Factory;
    using global::Hangfire.Console;
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
                var veeamVersion = GetInstalledVeeamVersion();

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

                    throw new Exception(result.Errors.ToString());
                }

               // Veeam payload = _veeamManager.GetLicensingInformation(performContext);
                Logger.Info(performContext, "done");

                Logger.Info(performContext, "Validating the payload...");
                var remoteVeeam = PortalService.GetVeeamServerById(AuthService.GetDevice());
                if (remoteVeeam.Count != 1)
                {
                    result.Value.Validate();
                    Logger.Info(performContext, "Payload is valid!");

                    DumpPayload(result.Value);

                    await PortalService.AddVeeamServerAsync(result.Value);

                    Logger.Info(performContext,"Successfully checked in.");

                    return;
                }

                remoteVeeam[0].UpdateValues(result.Value);
                remoteVeeam[0].Validate();

                Logger.Info(performContext, "Payload is valid!");

                DumpPayload(result.Value);

                await PortalService.UpdateVeeamServerAsync(remoteVeeam[0]);

                Logger.Info(performContext,"Successfully checked in.");
            });
        }

        public const string VeeamFilePath = @"C:\Program Files\Veeam\Backup and Replication\Backup\Veeam.Backup.Service.exe";
        public const string Veeam90FilePath = @"C:\Program Files\Veeam\Backup and Replication\Veeam.Backup.Service.exe";

        public Version GetInstalledVeeamVersion()
        {
            if (File.Exists(VeeamFilePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(VeeamFilePath);
                SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, version.FileVersion);
                return Version.Parse(version.FileVersion);
            }

            if (File.Exists(Veeam90FilePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(Veeam90FilePath);
                SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, version.FileVersion);
                return Version.Parse(version.FileVersion);
            }

            SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, string.Empty);
            return null;
        }

        private void DumpPayload(Veeam payload)
        {
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
        }
    }
}