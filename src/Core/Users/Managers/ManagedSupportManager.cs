//namespace LMS.Users.Managers
//{
//    using System;
//    using Abp.Configuration;
//    using Abp.Domain.Services;
//    using Abp.Timing;
//    using Common.Extensions;
//    using Common.Helpers;
//    using Configuration;
//    using Core.Common.Extensions;
//    using global::Hangfire.Server;
//    using OData;
//    using Portal.Common.Enums;
//    using Portal.LicenseMonitoringSystem.Users.Entities;

//    public class ManagedSupportManager : DomainService, IManagedSupportManager
//    {
//        private readonly IPortalManager _portalManager;

//        public ManagedSupportManager(IPortalManager portalManager)
//        {
//            _portalManager = portalManager;
//        }

//        public ManagedSupport Get()
//        {
//            var deviceId = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

//            int idOfManagedSupport = _portalManager.GetManagedSupportId(deviceId);

//            if (idOfManagedSupport == default(int))
//            {
//                return null;
//            }

//            SettingManager.ChangeSettingForApplication(AppSettingNames.ManagedSupportId, idOfManagedSupport.ToString());

//            return _portalManager.ListManagedSupportById(idOfManagedSupport);
//        }

//        public ManagedSupport Add(PerformContext performContext)
//        {
//            var deviceId = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

//            int uploadId = _portalManager.GenerateUploadId();

//            var ms = new ManagedSupport
//            {
//                CheckInTime = Clock.Now,
//                ClientVersion = SettingManagerHelper.ClientVersion,
//                DeviceId = deviceId,
//                Hostname = Environment.MachineName,
//                IsActive = true,
//                Status = CallInStatus.NotCalledIn,
//                UploadId = uploadId
//            };

//            _portalManager.AddManagedSupport(ms);
//            _portalManager.SaveChanges();

//            ManagedSupport managedSupport = Get();

//            Logger.Debug(performContext, $"Created new Managed Support {managedSupport.Id}");
//            SettingManager.ChangeSettingForApplication(AppSettingNames.ManagedSupportId, managedSupport.Id.ToString());
//            return managedSupport;
//        }

//        /// <inheritdoc />
//        public void Update(ManagedSupport input)
//        {
//            input.CheckInTime = new DateTimeOffset(Clock.Now);
//            input.ClientVersion = SettingManagerHelper.ClientVersion;
//            input.Hostname = Environment.MachineName;
//            input.Status = CallInStatus.CalledIn;
//            input.UploadId = _portalManager.GenerateUploadId();

//            _portalManager.UpdateManagedSupport(input);
//            _portalManager.SaveChanges();
//        }
//    }
//}