namespace LMS.Veeam
{
    using System;
    using System.Diagnostics;
    using Abp.Configuration;
    using Abp.Timing;
    using Castle.Core.Logging;
    using Common.Extensions;
    using Common.Managers;
    using Core.Configuration;
    using Managers;
    using OData;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamWorkerManager : LMSManagerBase, IVeeamWorkerManager
    {
        private readonly IPortalManager _portalManager;
        private readonly ISettingManager _settingManager;
        private readonly IVeeamManager _veeamManager;

        public VeeamWorkerManager(IPortalManager portalManager, ISettingManager settingManager, IVeeamManager veeamManager)
        {
            _portalManager = portalManager;
            _settingManager = settingManager;
            _veeamManager = veeamManager;
        }

        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info("Stopwatch started!");

            var deviceId = _settingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();
            Veeam veeam = _portalManager.ListVeeamById(deviceId);
            bool newVeeam = false;
            if (veeam == null)
            {
                veeam = new Veeam();
                newVeeam = true;
            }

            Logger.Info("Collecting information...this could take some time.");

            veeam = _veeamManager.GetLicensingInformation(veeam);
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

            stopWatch.Stop();
            Console.WriteLine(Environment.NewLine);
            Logger.Info($"Time elapsed: {stopWatch.Elapsed:hh\\:mm\\:ss}");
        }
    }
}