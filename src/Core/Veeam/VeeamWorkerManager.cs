namespace LMS.Veeam
{
    using System;
    using Abp.Configuration;
    using Abp.Timing;
    using Common.Extensions;
    using Common.Interfaces;
    using Common.Managers;
    using Core.Configuration;
    using global::Hangfire.Server;
    using Managers;
    using OData;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using Startup;

    public class VeeamWorkerManager : WorkerManagerBase, IVeeamWorkerManager
    {
        private readonly IPortalManager _portalManager;
        private readonly ISettingManager _settingManager;
        private readonly IStartupManager _startupManager;
        private readonly IVeeamManager _veeamManager;

        public VeeamWorkerManager(IPortalManager portalManager, ISettingManager settingManager, IVeeamManager veeamManager, IStartupManager startupManager)
        {
            _portalManager = portalManager;
            _settingManager = settingManager;
            _veeamManager = veeamManager;
            _startupManager = startupManager;
        }

        public override void Start(PerformContext performContext)
        {
            Execute(performContext, () =>
            {
                _startupManager.ValidateCredentials(performContext);

                var deviceId = _settingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();
                Veeam veeam = _portalManager.ListVeeamById(deviceId);
                bool newVeeam = false;
                if (veeam == null)
                {
                    veeam = new Veeam();
                    newVeeam = true;
                }

                Logger.Info(performContext, "Collecting information...this could take some time.");

                veeam = _veeamManager.GetLicensingInformation(performContext, veeam);
                veeam.Validate();

                Logger.Info(veeam.ToString());

                // set additional properties
                veeam.CheckInTime = new DateTimeOffset(Clock.Now);
                veeam.Status = CallInStatus.CalledIn;
                veeam.UploadId = _portalManager.GenerateUploadId();

                if (newVeeam)
                {
                    _portalManager.AddVeeam(veeam);
                }
                else
                {
                    _portalManager.UpdateVeeam(veeam);
                }
            });
        }
    }
}