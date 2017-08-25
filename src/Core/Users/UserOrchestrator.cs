namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Timing;
    using Administration;
    using Common.Client;
    using Common.Extensions;
    using Compare;
    using Models;
    using NLog;

    public class UserOrchestrator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly SettingManager SettingManager = new SettingManager();

        public async Task CallIn(int id)
        {
            var supportUploadClient = new SupportUploadClient();
            await supportUploadClient.Update(id);
        }

        public async Task ProcessGroups(List<LicenseUser> users)
        {
            Logger.Info(Environment.NewLine);
            var licenseGroupClient = new LicenseGroupClient();

            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();
            Logger.Info($"Local groups found: {localGroups.Count}");

            List<LicenseGroup> apiGroups = await licenseGroupClient.GetAll();

            List<LicenseGroup> groupsToCreate = apiGroups.FilterCreate<LicenseGroup, Guid>(localGroups);
            Logger.Info($"New Groups: {groupsToCreate.Count}");

            if (groupsToCreate.Count > 0)
            {
                await licenseGroupClient.Add(groupsToCreate);
            }

            List<LicenseGroup> groupsToUpdate = apiGroups.Except(localGroups).ToList();
            Logger.Info($"Update Groups: {groupsToUpdate.Count}");

            if (groupsToUpdate.Count > 0)
            {
                await licenseGroupClient.Update(groupsToUpdate);
            }

            var localGroupIds = new HashSet<Guid>(localGroups.Select(lg => lg.Id));
            List<LicenseGroup> groupsToDelete = apiGroups.Where(a => !localGroupIds.Contains(a.Id)).ToList();
            Logger.Info($"Delete Groups: {groupsToDelete.Count}");

            if (groupsToDelete.Count > 0)
            {
                await licenseGroupClient.Remove(groupsToDelete);
            }
        }

        public async Task ProcessUserGroups(List<LicenseUser> users)
        {
            Logger.Info(Environment.NewLine);
            var licenseUserClient = new LicenseUserClient();
            var licenseUserGroupClient = new LicenseUserGroupClient();

            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

            List<LicenseUser> apiUsers = await licenseUserClient.GetAll();

            foreach (LicenseUser apiUser in apiUsers)
            {
                if (apiUser.Groups != null)
                {
                    continue;
                }

                apiUser.Groups = new List<LicenseGroup>();
            }

            foreach (LicenseGroup localGroup in localGroups)
            {
                List<LicenseUser> usersThatWereMembers = apiUsers.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();
                List<LicenseUser> usersThatAreMembers = users.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();

                List<LicenseUser> usersToBeAdded = usersThatWereMembers.FilterCreate<LicenseUser, Guid>(usersThatAreMembers);

                if (usersToBeAdded.Any())
                {
                    await licenseUserGroupClient.Add(usersToBeAdded, localGroup);
                }

                var usersThatAreMembersIds = new HashSet<Guid>(usersThatAreMembers.Select(m => m.Id));
                List<LicenseUser> usersToBeRemoved = usersThatWereMembers.Where(u => !usersThatAreMembersIds.Contains(u.Id)).ToList();

                if (usersToBeRemoved.Any())
                {
                    await licenseUserGroupClient.Remove(usersToBeRemoved, localGroup);
                }

                if (usersToBeAdded.Any() || usersToBeRemoved.Any())
                {
                    Logger.Info($"Groups: {localGroup.Name}  Users Add: {usersToBeAdded.Count} Users Remove: {usersToBeRemoved.Count}");
                }
            }
        }

        public async Task<List<LicenseUser>> ProcessUsers(int uploadId)
        {
            Logger.Info(Environment.NewLine);
            var licenseUserClient = new LicenseUserClient();
            var supportUploadClient = new SupportUploadClient();
            var userManager = new UserManager();

            List<LicenseUser> localUsers = userManager.GetUsersAndGroups();
            Logger.Info($"Local users found: {localUsers.Count}");

            List<LicenseUser> apiUsers = await supportUploadClient.GetUsers(uploadId);

            var newUsers = GetUsersToCreate(localUsers, apiUsers, uploadId);
            Logger.Info($"New Users: {newUsers.Count}");

            if (newUsers.Count > 0)
            {                
                await licenseUserClient.Add(newUsers);
            }


            List<LicenseUser> usersToUpdate = GetUsersToUpdate(localUsers, apiUsers);
            Logger.Info($"Update Users: {usersToUpdate.Count}");

            if (usersToUpdate.Count > 0)
            {
                await licenseUserClient.Update(usersToUpdate);
            }


            List<LicenseUser> usersToDelete = GetUsersToDelete(localUsers, apiUsers);
            Logger.Info($"Delete Users: {usersToDelete.Count}");

            if (usersToDelete.Count > 0)
            {
                await licenseUserClient.Remove(usersToDelete);
            }

            return localUsers;
        }

        protected List<LicenseUser> GetUsersToDelete(List<LicenseUser> localUsers, List<LicenseUser> apiUsers)
        {
            if (localUsers == null || apiUsers == null)
            {
                return new List<LicenseUser>();
            }

            if (apiUsers.Count == 0)
            {
                return new List<LicenseUser>();
            }

            var localUserIds = new HashSet<Guid>(localUsers.Select(lu => lu.Id));

            apiUsers.RemoveAll(au => localUserIds.Contains(au.Id));

            return apiUsers;
        }

        protected List<LicenseUser> GetUsersToUpdate(List<LicenseUser> localUsers, List<LicenseUser> apiUsers)
        {
            if (localUsers == null || apiUsers == null || apiUsers.Count == 0)
            {
                return new List<LicenseUser>();
            }

            var usersToUpdate = new List<LicenseUser>();
            var compLogic = new LicenseUserCompareLogic();

            foreach (var apiUser in apiUsers)
            {
                var localUser = localUsers.FirstOrDefault(lu => lu.Id == apiUser.Id);
                if (localUser == null)
                {
                    continue;
                }

                var result = compLogic.Compare(localUser, apiUser);
                if (!result.AreEqual)
                {
                    usersToUpdate.Add(localUser);
                }
            }

            return usersToUpdate;
        }

        protected List<LicenseUser> GetUsersToCreate(List<LicenseUser> localUsers, List<LicenseUser> apiUsers, int uploadId)
        {
            if (localUsers == null)
            {
                return new List<LicenseUser>();
            }

            List<LicenseUser> newUsers;

            if (apiUsers != null && apiUsers.Count > 0)
            {
                newUsers = localUsers.Where(lu => apiUsers.All(au => au.Id != lu.Id)).ToList();
            }
            else
            {
                newUsers = localUsers;
            }

            if (newUsers.Count > 0)
            {
                newUsers.ForEach(lu => lu.ManagedSupportId = uploadId);
            }

            return newUsers;   
        }

        public async Task Start()
        {
            var supportUploadClient = new SupportUploadClient();

            Guid deviceId = await SettingManager.GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);

            int? managedSupportId = await supportUploadClient.GetIdByDeviceId(deviceId);
            if (managedSupportId == null)
            {
                int uploadId = await supportUploadClient.GetNewUploadId();
                ManagedSupport managedSupport = await supportUploadClient.Add(new ManagedSupport
                {
                    CheckInTime = Clock.Now,
                    DeviceId = deviceId,
                    Hostname = Environment.MachineName,
                    IsActive = true,
                    Status = CallInStatus.NotCalledIn,
                    UploadId = uploadId
                });

                managedSupportId = managedSupport.Id;
            }

            Logger.Info("Collecting information...this could take some time.");

            List<LicenseUser> users = await ProcessUsers((int) managedSupportId);

            await ProcessGroups(users);

            await ProcessUserGroups(users);

            await CallIn((int) managedSupportId);
        }
    }
}