using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Abp.Timing;
    using Common.Extensions;
    using Common.Helpers;
    using Core.Configuration;
    using Core.OData;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class ManagedSupportManager : DomainService, IManagedSupportManager
    {
        private readonly PortalClient _portalClient;
        public ManagedSupportManager(PortalClient portalClient)
        {
            _portalClient = portalClient;
        }

        public ManagedSupport Get()
        {
            Guid deviceId = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

            int idOfManagedSupport = _portalClient.GetManagedSupportId(deviceId);

            if (idOfManagedSupport == default(int))
            {
                return null;
            }

            return _portalClient.ListManagedSupportById(idOfManagedSupport);
        }

        public ManagedSupport Add()
        {
            Guid deviceId = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

            int uploadId = _portalClient.GenerateUploadId();

            var ms = new ManagedSupport
            {
                CheckInTime = Clock.Now,
                ClientVersion = SettingManagerHelper.Instance.ClientVersion,
                DeviceId = deviceId,
                Hostname = Environment.MachineName,
                IsActive = true,
                Status = CallInStatus.NotCalledIn,
                UploadId = uploadId
            };

            _portalClient.AddManagedSupport(ms);
            _portalClient.SaveChanges();

            var managedSupport = Get();

            Logger.Debug($"Created new Managed Support {managedSupport.Id}");
            return managedSupport;
        }

        /// <inheritdoc />
        public void Update(ManagedSupport input)
        {
            input.CheckInTime = new DateTimeOffset(Clock.Now);
            input.ClientVersion = SettingManagerHelper.Instance.ClientVersion;
            input.Hostname = Environment.MachineName;
            input.Status = CallInStatus.CalledIn;
            input.UploadId = _portalClient.GenerateUploadId();

            _portalClient.UpdateManagedSupport(input);
            _portalClient.SaveChanges();

            Logger.Info("Successfully called in.");
        }
    }
}
