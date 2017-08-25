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
    using Entities;
    using KellermanSoftware.CompareNetObjects;
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

            List<LicenseGroup> groupsToCreate = GetUsersOrGroupsToCreate(localGroups, apiGroups);
            Logger.Info($"New Groups: {groupsToCreate.Count}");

            if (groupsToCreate.Count > 0)
            {
                await licenseGroupClient.Add(groupsToCreate);
            }


            List<LicenseGroup> groupsToUpdate = GetUsersOrGroupsToUpdate<LicenseGroup, CompareLogic>(localGroups, apiGroups);
            Logger.Info($"Update Groups: {groupsToUpdate.Count}");

            if (groupsToUpdate.Count > 0)
            {
                await licenseGroupClient.Update(groupsToUpdate);
            }


            List<LicenseGroup> groupsToDelete = GetUsersOrGroupsToDelete(localGroups, apiGroups);
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
                List<LicenseUser> usersThatWereMembers = apiUsers.Where(u => u.Groups.Any(g => g.Id == localGroup.Id)).ToList();
                List<LicenseUser> usersThatAreMembers = users.Where(u => u.Groups.Any(g => g.Id == localGroup.Id)).ToList();

                List<LicenseUser> usersToBeAdded = GetUsersOrGroupsToCreate(usersThatWereMembers, usersThatAreMembers);

                if (usersToBeAdded.Count > 0)
                {
                    await licenseUserGroupClient.Add(usersToBeAdded, localGroup);
                }

                List<LicenseUser> usersToBeRemoved = GetUsersOrGroupsToDelete(usersThatAreMembers, usersThatWereMembers);

                if (usersToBeRemoved.Count > 0)
                {
                    await licenseUserGroupClient.Remove(usersToBeRemoved, localGroup);
                }

                if (usersToBeAdded.Count > 0 || usersToBeRemoved.Count > 0)
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

            var newUsers = GetUsersOrGroupsToCreate(localUsers, apiUsers, uploadId);
            Logger.Info($"New Users: {newUsers.Count}");

            if (newUsers.Count > 0)
            {
                await licenseUserClient.Add(newUsers);
            }


            List<LicenseUser> usersToUpdate = GetUsersOrGroupsToUpdate<LicenseUser, LicenseUserCompareLogic>(localUsers, apiUsers);
            Logger.Info($"Update Users: {usersToUpdate.Count}");

            if (usersToUpdate.Count > 0)
            {
                await licenseUserClient.Update(usersToUpdate);
            }


            List<LicenseUser> usersToDelete = GetUsersOrGroupsToDelete(localUsers, apiUsers);
            Logger.Info($"Delete Users: {usersToDelete.Count}");

            if (usersToDelete.Count > 0)
            {
                await licenseUserClient.Remove(usersToDelete);
            }

            return localUsers;
        }

        protected List<TEntity> GetUsersOrGroupsToDelete<TEntity>(List<TEntity> localEntities, List<TEntity> apiEntities)
            where TEntity : LicenseBase
        {
            if (localEntities == null || apiEntities == null)
            {
                return new List<TEntity>();
            }

            if (apiEntities.Count == 0)
            {
                return new List<TEntity>();
            }

            var localEntityIds = new HashSet<Guid>(localEntities.Select(lu => lu.Id));

            apiEntities.RemoveAll(au => localEntityIds.Contains(au.Id));

            return apiEntities;
        }

        protected List<TEntity> GetUsersOrGroupsToUpdate<TEntity, TCompareLogic>(List<TEntity> localEntities, List<TEntity> apiEntities)
            where TEntity : LicenseBase
            where TCompareLogic : CompareLogic, new()
        {
            if (localEntities == null || apiEntities == null || apiEntities.Count == 0)
            {
                return new List<TEntity>();
            }

            var entitiesToUpdate = new List<TEntity>();
            var compLogic = new TCompareLogic();

            foreach (var apiEntity in apiEntities)
            {
                var localEntity = localEntities.FirstOrDefault(lu => lu.Id == apiEntity.Id);
                if (localEntity == null)
                {
                    continue;
                }

                var result = compLogic.Compare(localEntity, apiEntity);
                if (!result.AreEqual)
                {
                    Logger.Debug($"Entity: {localEntity.Id} requires an update.");
                    Logger.Debug(result.DifferencesString);
                    entitiesToUpdate.Add(localEntity);                  
                }
            }

            return entitiesToUpdate;
        }

        protected List<TEntity> GetUsersOrGroupsToCreate<TEntity>(List<TEntity> localEntities, List<TEntity> apiEntities, int uploadId = 0)
            where TEntity : LicenseBase
        {
            if (localEntities == null)
            {
                return new List<TEntity>();
            }

            List<TEntity> newEntities;

            if (apiEntities != null && apiEntities.Count > 0)
            {
                newEntities = localEntities.Where(lu => apiEntities.All(au => au.Id != lu.Id)).ToList();
            }
            else
            {
                newEntities = localEntities;
            }

            if (typeof(TEntity) != typeof(LicenseUser))
            {
                return newEntities;
            }

            if (newEntities.Count <= 0)
            {
                return newEntities;
            }

            if (uploadId == default(int))
            {
                return newEntities;
            }

            List<LicenseUser> newUsers = newEntities.Cast<LicenseUser>().ToList();
            newUsers.ForEach(lu => lu.ManagedSupportId = uploadId);

            return newUsers.Cast<TEntity>().ToList();
        }

        public async Task Start()
        {
            var supportUploadClient = new SupportUploadClient();

            Guid deviceId = await SettingManager.GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);

            int managedSupportId = await supportUploadClient.GetIdByDeviceId(deviceId);
            if (managedSupportId == default(int))
            {
                int uploadId = await supportUploadClient.GetNewUploadId();
                Logger.Info("Upload ID: " + uploadId.Dump());
                ManagedSupport managedSupport = await supportUploadClient.Add(new ManagedSupport
                {
                    CheckInTime = Clock.Now,
                    ClientVersion = SettingManager.GetClientVersion(),
                    DeviceId = deviceId,
                    Hostname = Environment.MachineName,
                    IsActive = true,
                    Status = CallInStatus.NotCalledIn,
                    UploadId = uploadId
                });

                Logger.Debug(managedSupport.Dump());
                managedSupportId = managedSupport.Id;
            }

            Logger.Info("Collecting information...this could take some time.");

            List<LicenseUser> users = await ProcessUsers(managedSupportId);

            await ProcessGroups(users);

            await ProcessUserGroups(users);

            await CallIn(managedSupportId);
        }
    }
}