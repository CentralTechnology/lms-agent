namespace Core.Veeam
{
    using System;
    using System.Diagnostics;
    using Abp.Timing;
    using Common.Extensions;
    using Common.Helpers;
    using Models;
    using NLog;
    using OData;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamOrchestrator : IDisposable
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        protected PortalClient PortalClient = new PortalClient();

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    PortalClient = null;
                }
            }

            _disposed = true;
        }

        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info("Stopwatch started!");

            Guid deviceId = SettingManagerHelper.Instance.DeviceId;
            Veeam veeam = PortalClient.ListVeeamById(deviceId);
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
            veeam.UploadId = PortalClient.GenerateUploadId();

            if (newVeeam)
            {
                PortalClient.AddVeeam(veeam);
            }
            else
            {
                PortalClient.UpdateVeeam(veeam);
            }

            stopWatch.Stop();
            Console.WriteLine(Environment.NewLine);
            Logger.Info($"Time elapsed: {stopWatch.Elapsed:hh\\:mm\\:ss}");
        }
    }
}