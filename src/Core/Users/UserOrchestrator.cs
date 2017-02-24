using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Users
{
    using Common.Client;
    using Common.Extensions;
    using Models;

    public class UserOrchestrator : LicenseMonitoringBase, IUserOrchestrator
    {
        private readonly ISupportUploadClient _uploadClient;
        private readonly ILicenseUserClient _licenseUserClient;
        private readonly IUserManager _userManager;
        public UserOrchestrator(ISupportUploadClient uploadClient, ILicenseUserClient licenseUserClient, IUserManager userManager)
        {
            _uploadClient = uploadClient;
            _licenseUserClient = licenseUserClient;
            _userManager = userManager;
        }
        public async Task<int> ProcessUpload()
        {
            var deviceId = SettingManager.GetDeviceId();

            var status = await _uploadClient.GetStatusByDeviceId(deviceId);

            if (status.HasFlag(CallInStatus.CalledIn))
            {
                return 0;
            }

            var id = await _uploadClient.GetUploadIdByDeviceId(deviceId);
            return id;
        }

        public async Task<List<LicenseUser>> ProcessUsers(int uploadId)
        {
            var localUsers = _userManager.GetUsersAndGroups();
            var remoteUsers = await _uploadClient.GetUsers(uploadId);

            var usersToCreate = localUsers.FilterMissing(remoteUsers);
            usersToCreate = usersToCreate.ApplyUploadId(uploadId);
            await _licenseUserClient.Add(usersToCreate);

            var usersToUpdate = remoteUsers.FilterExisting(localUsers);
            await _licenseUserClient.Update(usersToUpdate);

            var usersToDelete = remoteUsers.FilterMissing(localUsers);
            await _licenseUserClient.Remove(usersToDelete);

            return localUsers;

        }

        public async Task ProcessGroups(List<LicenseUser> users)
        {
            throw new NotImplementedException();
        }

        public async Task ProcessUserGroups(List<LicenseUser> users)
        {
            throw new NotImplementedException();
        }
    }
}
