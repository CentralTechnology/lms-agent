namespace LicenseMonitoringSystem.Core
{
    using Abp.Dependency;
    using Abp.Timing;
    using Common.Client;
    using Common.Extensions;
    using Common.Portal.License.User;
    using Settings;
    using Users;

    public class Orchestrator : LicenseMonitoringBase, ISingletonDependency
    {
        private readonly IPortalClient _portalClient;
        private readonly IUserManager _userManager;

        public Orchestrator(
            IUserManager userManager,
            IPortalClient portalClient,
            SettingManager settingManager)
            : base(settingManager)
        {
            _userManager = userManager;
            _portalClient = portalClient;
        }

        private LicenseUserUpload CreateLicenseUserUpload(int uploadId)
        {
            return new LicenseUserUpload
            {
                CheckInTime = Clock.Now,
                DeviceId = SettingManager.DeviceId,
                TenantId = SettingManager.AccountId,
                Users = _userManager.GetUsersAndGroups().Convert(),
                UploadId = uploadId
            };
        }

        public void Run(Monitor monitor)
        {
            switch (monitor)
            {
                case Monitor.Users:
                    Logger.Info("Monitoring Users");
                    Users();

                    break;
                default:
                    Logger.Error("No monitors selected. Please check the settings.json file.");
                    break;
            }
        }

        private LicenseUserUpload UpdateLicenseUserUpload(LicenseUserUpload upload)
        {
            upload.CheckInTime = Clock.Now;
            upload.Users = _userManager.GetUsersAndGroups().Convert();

            return upload;
        }

        private void Users()
        {
            var status = _portalClient.GetStatus(SettingManager.DeviceId);

            if (status == Common.Portal.Common.Enums.CallInStatus.CalledIn)
            {
                Logger.Info("Upload status is set to: CalledIn.");
                Logger.Info("Will try again later.");
                return;
            }

            Logger.Info("Upload status is set to: NotCalledIn.");
            Logger.Info("CheckIn required.");

            var uploadId = _portalClient.GetId(SettingManager.DeviceId);
            if (uploadId == 0)
            {
                var upload = CreateLicenseUserUpload(uploadId);
                _portalClient.Post(upload);
            }
            else
            {
                var upload = _portalClient.Get(uploadId);
                upload = UpdateLicenseUserUpload(upload);
                _portalClient.Put(upload.Id, upload);
            }
        }
    }
}