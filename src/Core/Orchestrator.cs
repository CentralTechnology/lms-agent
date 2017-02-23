namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abp.AutoMapper;
    using Abp.Dependency;
    using Abp.Timing;
    using Common.Client;
    using Common.Extensions;
    using LicenseMonitoringSystem.Core;
    using LicenseMonitoringSystem.Core.Common.Portal.Common.Enums;
    using LicenseMonitoringSystem.Core.Common.Portal.License.User;
    using Settings;
    using Users;

    public class Orchestrator : LicenseMonitoringBase, ISingletonDependency
    {
        private readonly IUserClient _userClient;
        private readonly IUserGroupClient _userGroupClient;
        private readonly IUserManager _userManager;
        private readonly IUserUploadClient _userUploadClient;

        public Orchestrator(
            IUserManager userManager,
            IUserClient userClient,
            IUserGroupClient userGroupClient,
            IUserUploadClient userUploadClient,
            SettingManager settingManager)
            : base(settingManager)
        {
            _userManager = userManager;
            _userClient = userClient;
            _userGroupClient = userGroupClient;
            _userUploadClient = userUploadClient;
        }

        /// <summary>
        /// </summary>
        /// <param name="uploadId"></param>
        /// <returns></returns>
        protected LicenseUserUpload CreateLicenseUserUpload(int uploadId)
        {
            var upload = new LicenseUserUpload
            {
                CheckInTime = Clock.Now,
                DeviceId = SettingManager.GetDeviceId(),
                TenantId = SettingManager.GetAccountId(),
                UploadId = uploadId
            };

            return upload;
        }

        protected List<LicenseUserGroup> FilterExistingGroups(List<UserGroup> newGroups, List<LicenseUserGroup> originalGroups)
        {
            // need to map to difference type for comparison
            var oGroups = originalGroups.MapTo<List<UserGroup>>();

            // compare
            var groupsToUpdate = oGroups.FilterExisting<UserGroup, Guid>(newGroups).Select(ug => ug.Id);

            // return the correct type needed for the api
            return newGroups.Where(ug => groupsToUpdate.Contains(ug.Id)).ToList().MapTo<List<LicenseUserGroup>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="newUsers"></param>
        /// <param name="originalUsers"></param>
        /// <returns></returns>
        private List<LicenseUser> FilterExistingUsers(List<User> newUsers, List<LicenseUser> originalUsers)
        {
            // need to map to different type for comparison
            var oUsers = originalUsers.MapTo<List<User>>();

            // compare
            var usersToUpdate = oUsers.FilterExisting<User, Guid>(newUsers).Select(u => u.Id);

            // return as the correct type needed for the api
            return newUsers.Where(u => usersToUpdate.Contains(u.Id)).ToList().MapTo<List<LicenseUser>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="newGroups"></param>
        /// <param name="originalGroups"></param>
        /// <returns></returns>
        protected List<LicenseUserGroup> FilterNewGroups(List<UserGroup> newGroups, List<LicenseUserGroup> originalGroups)
        {
            // need to map to different type for comparison
            var oGroups = originalGroups.MapTo<List<UserGroup>>();

            // compare
            var groupsToCreate = newGroups.FilterMissing<UserGroup, Guid>(oGroups);

            // return as the correct type needed for the api
            return groupsToCreate.MapTo<List<LicenseUserGroup>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="newUsers"></param>
        /// <param name="originalUsers"></param>
        /// <returns></returns>
        protected List<LicenseUser> FilterNewUsers(List<User> newUsers, List<LicenseUser> originalUsers)
        {
            // need to map to different type for comparison
            var oUsers = originalUsers.MapTo<List<User>>();

            // compare
            var usersToCreate = newUsers.FilterMissing<User, Guid>(oUsers);

            // return as the correct type needed for the api
            return usersToCreate.MapTo<List<LicenseUser>>();
        }

        protected List<LicenseUserGroup> FilterStaleGroups(List<UserGroup> newGroups, List<LicenseUserGroup> originalGroups)
        {
            // need to map to different type for comparison
            var oGroups = originalGroups.MapTo<List<UserGroup>>();

            // compare
            var groupsToDelete = oGroups.FilterMissing<UserGroup, Guid>(newGroups);

            // return as the correct type needed for the api
            return groupsToDelete.MapTo<List<LicenseUserGroup>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="newUsers"></param>
        /// <param name="originalUsers"></param>
        /// <returns></returns>
        protected List<LicenseUser> FilterStaleUsers(List<User> newUsers, List<LicenseUser> originalUsers)
        {
            // need to map to different type for comparison
            var oUsers = originalUsers.MapTo<List<User>>();

            // compare
            var usersToDelete = oUsers.FilterMissing<User, Guid>(newUsers);

            // return as the correct type needed for the api
            return usersToDelete.MapTo<List<LicenseUser>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="users"></param>
        /// <param name="apiUsers"></param>
        protected void ProcessGroups(List<User> users, List<LicenseUser> apiUsers)
        {
            var groups = users.SelectMany(u => u.Groups).ToList();
            var apiGroups = apiUsers.SelectMany(u => u.Groups).ToList();

            // create
            var groupsToCreate = FilterNewGroups(groups, apiGroups);

            Logger.InfoFormat("There are {0} user groups that need creating.", groupsToCreate.Count);

            if (groupsToCreate.Count > 0)
            {
                _userGroupClient.Add(groupsToCreate);
            }

            // update
            var groupsToUpdate = FilterExistingGroups(groups, apiGroups);

            Logger.InfoFormat("There are {0} user groups that need updating.", groupsToUpdate.Count);

            if (groupsToUpdate.Count > 0)
            {
                _userGroupClient.Update(groupsToUpdate);
            }

            // delete
            var groupsToDelete = FilterStaleGroups(groups, apiGroups);

            Logger.InfoFormat("There are {0} user groups that need deleting.", groupsToUpdate.Count);

            if (groupsToDelete.Count > 0)
            {
                _userGroupClient.Remove(groupsToDelete);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="users"></param>
        /// <param name="apiUsers"></param>
        /// <param name="uploadId"></param>
        protected void ProcessUsers(List<User> users, List<LicenseUser> apiUsers, int uploadId)
        {
            // create
            var usersToCreate = FilterNewUsers(users, apiUsers);
            usersToCreate.ApplyUploadId(uploadId);

            Logger.InfoFormat("There are {0} users that need creating.", usersToCreate.Count);

            if (usersToCreate.Count > 0)
            {
                _userClient.Add(usersToCreate);
            }

            // update
            var usersToUpdate = FilterExistingUsers(users, apiUsers);
            usersToUpdate.ApplyUploadId(uploadId);

            Logger.InfoFormat("There are {0} users that need updating.", usersToUpdate.Count);

            if (usersToUpdate.Count > 0)
            {
                _userClient.Update(usersToUpdate);
            }

            // delete
            var usersToDelete = FilterStaleUsers(users, apiUsers);

            Logger.InfoFormat("There are {0} users that need deleting", usersToDelete.Count);

            if (usersToDelete.Count > 0)
            {
                _userClient.Remove(usersToDelete);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="monitor"></param>
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

        /// <summary>
        /// </summary>
        protected void Users()
        {
            var deviceId = SettingManager.GetDeviceId();
            var status = _userUploadClient.GetStatusByDeviceId(deviceId);

            if (status == CallInStatus.CalledIn)
            {
                Logger.Info("Upload status is set to: CalledIn.");
                Logger.Info("Will try again later.");
                return;
            }

            Logger.Info("Upload status is set to: NotCalledIn.");
            Logger.Info("CheckIn required.");

            var uploadId = _userUploadClient.GetUploadIdByDeviceId(deviceId);
            LicenseUserUpload upload;
            if (uploadId == 0)
            {
                upload = CreateLicenseUserUpload(uploadId);
                _userUploadClient.Add(upload);
            }
            else
            {
                upload = _userUploadClient.Get(uploadId);

                upload.CheckInTime = Clock.Now;
                upload.Status = CallInStatus.CalledIn;
                upload.Hostname = Environment.MachineName;
            }

            var users = _userManager.GetUsersAndGroups();

            ProcessUsers(users, upload.Users.ToList(), upload.Id);

            ProcessGroups(users, upload.Users.ToList());

            Logger.Info("Complete!");
        }
    }
}