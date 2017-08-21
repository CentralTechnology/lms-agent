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
    using Models;
    using NLog;

    public class UserOrchestrator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly SupportUploadClient SupportUploadClient = new SupportUploadClient();
        private static readonly SettingManager SettingManager = new SettingManager();
        private static readonly UserManager UserManager = new UserManager();
        private static readonly LicenseUserClient LicenseUserClient = new LicenseUserClient();
        private static readonly LicenseGroupClient LicenseGroupClient = new LicenseGroupClient();
        private static readonly LicenseUserGroupClient LicenseUserGroupClient = new LicenseUserGroupClient();

        public async Task CallIn(int id)
        {
            await SupportUploadClient.Update(id);
        }

        public async Task ProcessGroups(List<LicenseUser> users)
        {
            Logger.Info(Environment.NewLine);

            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();
            Logger.Info($"Local groups found: {localGroups.Count}");

            List<LicenseGroup> apiGroups = await LicenseGroupClient.GetAll();

            List<LicenseGroup> groupsToCreate = apiGroups.FilterCreate<LicenseGroup, Guid>(localGroups);
            Logger.Info($"New Groups: {groupsToCreate.Count}");

            if (groupsToCreate.Count > 0)
            {
                await LicenseGroupClient.Add(groupsToCreate);
            }

            List<LicenseGroup> groupsToUpdate = apiGroups.Except(localGroups).ToList();
            Logger.Info($"Update Groups: {groupsToUpdate.Count}");

            if (groupsToUpdate.Count > 0)
            {
                await LicenseGroupClient.Update(groupsToUpdate);
            }

            var localGroupIds = new HashSet<Guid>(localGroups.Select(lg => lg.Id));
            List<LicenseGroup> groupsToDelete = apiGroups.Where(a => !localGroupIds.Contains(a.Id)).ToList();
            Logger.Info($"Delete Groups: {groupsToDelete.Count}");

            if (groupsToDelete.Count > 0)
            {
                await LicenseGroupClient.Remove(groupsToDelete);
            }
        }

        public async Task ProcessUserGroups(List<LicenseUser> users)
        {
            Logger.Info(Environment.NewLine);

            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

            List<LicenseUser> apiUsers = await LicenseUserClient.GetAll();

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

                if (usersToBeAdded.Count > 0)
                {
                    await LicenseUserGroupClient.Add(usersToBeAdded, localGroup);
                }

                var usersThatAreMembersIds = new HashSet<Guid>(usersThatAreMembers.Select(m => m.Id));
                List<LicenseUser> usersToBeRemoved = usersThatWereMembers.Where(u => !usersThatAreMembersIds.Contains(u.Id)).ToList();

                if (usersToBeRemoved.Count > 0)
                {
                    await LicenseUserGroupClient.Remove(usersToBeRemoved, localGroup);
                }
            }
        }

        public async Task<List<LicenseUser>> ProcessUsers(int uploadId)
        {
            Logger.Info(Environment.NewLine);

            List<LicenseUser> localUsers = UserManager.GetUsersAndGroups();
            Logger.Info($"Local users found: {localUsers.Count}");

            List<LicenseUser> apiUsers = await SupportUploadClient.GetUsers(uploadId);

            List<LicenseUser> usersToCreate = apiUsers.FilterCreate<LicenseUser, Guid>(localUsers);
            Logger.Info($"New Users: {usersToCreate.Count}");

            if (usersToCreate.Any())
            {
                List<LicenseUser> newUsers = ListExtensions.ApplyUploadId(usersToCreate, uploadId);
                await LicenseUserClient.Add(newUsers);
            }

            List<LicenseUser> usersToUpdate = apiUsers.Except(localUsers).ToList();
            Logger.Info($"Update Users: {usersToUpdate.Count}");

            if (usersToUpdate.Count > 0)
            {
                await LicenseUserClient.Update(usersToUpdate);
            }

            var localUserIds = new HashSet<Guid>(localUsers.Select(lu => lu.Id));
            List<LicenseUser> usersToDelete = apiUsers.Where(a => !localUserIds.Contains(a.Id)).ToList();
            Logger.Info($"Delete Users: {usersToDelete}");

            if (usersToDelete.Count > 0)
            {
                await LicenseUserClient.Remove(usersToDelete);
            }

            return localUsers;
        }

        public async Task Start()
        {
            Guid deviceId = await SettingManager.GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);

            CallInStatus status = await SupportUploadClient.GetStatusByDeviceId(deviceId);

            status.FriendlyMessage(Logger);
            if (status == CallInStatus.CalledIn)
            {
                return;
            }

            int? managedSupportId = await SupportUploadClient.GetIdByDeviceId(deviceId);
            if (managedSupportId == null)
            {
                var uploadId = await SupportUploadClient.GetNewUploadId();
                var managedSupport = await SupportUploadClient.Add(new ManagedSupport
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