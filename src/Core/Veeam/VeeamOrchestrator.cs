namespace LMS.Veeam
{
    using System;
    using System.Diagnostics;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Timing;
    using Castle.Core.Logging;
    using Core.Common.Extensions;
    using Core.Common.Helpers;
    using Core.Configuration;
    using Core.OData;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamOrchestrator : ITransientDependency
    {
        public ILogger Logger { get; set; }
        private readonly PortalClient _portalClient;
        private readonly ISettingManager _settingManager;

        public VeeamOrchestrator(PortalClient portalClient, ISettingManager settingManager)
        {
            Logger = NullLogger.Instance;
            _portalClient = portalClient;
            _settingManager = settingManager;
        }


        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info("Stopwatch started!");

            Guid deviceId = _settingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();
            Veeam veeam = _portalClient.ListVeeamById(deviceId);
            bool newVeeam = false;
            if (veeam == null)
            {
                veeam = new Veeam();
                newVeeam = true;
            }

            Logger.Info("Collecting information...this could take some time.");

            veeam.CollectInformation();

            Logger.Info($"Edition: {veeam.Edition}  License: {veeam.LicenseType}  Version: {veeam.ProgramVersion}  Hyper-V: {veeam.HyperV}  VMWare: {veeam.vSphere}");

            // set additional properties
            veeam.CheckInTime = new DateTimeOffset(Clock.Now);
            veeam.Status = CallInStatus.CalledIn;
            veeam.UploadId = _portalClient.GenerateUploadId();

            if (newVeeam)
            {
                _portalClient.AddVeeam(veeam);
            }
            else
            {
                _portalClient.UpdateVeeam(veeam);
            }

            stopWatch.Stop();
            Console.WriteLine(Environment.NewLine);
            Logger.Info($"Time elapsed: {stopWatch.Elapsed:hh\\:mm\\:ss}");
        }
    }
}