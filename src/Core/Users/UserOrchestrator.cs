namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Domain.Entities;
    using Abp.Timing;
    using Administration;
    using Common.Client;
    using Common.Extensions;
    using Common.Helpers;
    using Compare;
    using Entities;
    using KellermanSoftware.CompareNetObjects;
    using Models;
    using NLog;
    using ServiceStack.Text;
    using Simple.OData.Client;

    public class UserOrchestrator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly SettingManager SettingManager = new SettingManager();

        public async Task CallIn(int id)
        {
            var supportUploadClient = new SupportUploadClient();
            await supportUploadClient.Update(id);
        }

        protected List<TEntity> GetUsersOrGroupsToCreate<TEntity>(IEnumerable<TEntity> localEntities, IList<TEntity> apiEntities, int uploadId = 0)
            where TEntity : LicenseBase
        {
            if (localEntities == null)
            {
                return new List<TEntity>();
            }
           
            List<TEntity> newEntities;

            if (apiEntities != null && apiEntities.Any())
            {
                newEntities = localEntities.Where(lu => apiEntities.All(au => au.Id != lu.Id)).ToList();
            }
            else
            {
                newEntities = new List<TEntity>(localEntities);
            }

            if (typeof(TEntity) != typeof(LicenseUser))
            {
                return newEntities;
            }

            if (!newEntities.Any())
            {
                return newEntities;
            }

            if (uploadId == default(int))
            {
                return newEntities;
            }

            List<LicenseUser> newUsers = new List<LicenseUser>(newEntities.Cast<LicenseUser>());
            newUsers.ForEach(lu => lu.ManagedSupportId = uploadId);

            return newUsers.Cast<TEntity>().ToList();
        }

        protected List<TEntity> GetUsersOrGroupsToDelete<TEntity>(IEnumerable<TEntity> localEntities, List<TEntity> apiEntities)
            where TEntity : LicenseBase
        {
            if (localEntities == null || apiEntities == null)
            {
                return new List<TEntity>();
            }

            if (!apiEntities.Any())
            {
                return new List<TEntity>();
            }

            var localEntityIds = new HashSet<Guid>(localEntities.Select(lu => lu.Id));

            apiEntities.RemoveAll(au => localEntityIds.Contains(au.Id));

            return apiEntities;
        }

        protected List<TEntity> GetUsersOrGroupsToUpdate<TEntity, TCompareLogic>(IList<TEntity> localEntities, IList<TEntity> apiEntities)
            where TEntity : LicenseBase
            where TCompareLogic : CompareLogic, new()
        {
            if (localEntities == null || apiEntities == null || !apiEntities.Any())
            {
                return new List<TEntity>();
            }

            var entitiesToUpdate = new List<TEntity>();
            var compLogic = new TCompareLogic();

            foreach (TEntity apiEntity in apiEntities)
            {
                TEntity localEntity = localEntities.FirstOrDefault(lu => lu.Id == apiEntity.Id);
                if (localEntity == null)
                {
                    continue;
                }

                ComparisonResult result = compLogic.Compare(localEntity, apiEntity);
                if (!result.AreEqual)
                {
                    Logger.Debug($"Entity: {localEntity.Id} requires an update.");
                    Logger.Debug(result.DifferencesString);
                    entitiesToUpdate.Add(localEntity);
                }
            }

            return entitiesToUpdate;
        }

        public async Task ProcessGroups(List<LicenseUser> users)
        {
            Logger.Info(Environment.NewLine);
            var licenseGroupClient = new LicenseGroupClient();

            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();
            Logger.Info($"Local groups found: {localGroups.Count}");

            IEnumerable<LicenseGroup> allGroups = await licenseGroupClient.GetAll();
            var apiGroups = new List<LicenseGroup>(allGroups);

            List<LicenseGroup> groupsToCreate = GetUsersOrGroupsToCreate(localGroups, apiGroups);
            Logger.Info($"New Groups: {groupsToCreate.Count}");

            if (groupsToCreate.Count > 0)
            {
                await licenseGroupClient.Add(groupsToCreate);
            }

            List<LicenseGroup> groupsToUpdate = GetUsersOrGroupsToUpdate<LicenseGroup, CompareLogic>(localGroups, apiGroups);
            Logger.Info($"Update Groups: {groupsToUpdate.Count}");

            if (groupsToUpdate.Any())
            {
                await licenseGroupClient.Update(groupsToUpdate);
            }

            /*
             * We need groups that are not already deleted. Otherwise we will be deleting groups that are already deleted,
             * wasting api calls.
             */
            IEnumerable<LicenseGroup> deleteGroups = await licenseGroupClient.GetAll(new ODataExpression<LicenseGroup>(lg => !lg.IsDeleted));
            List<LicenseGroup> groupsToDelete = GetUsersOrGroupsToDelete(localGroups, deleteGroups.ToList());
            Logger.Info($"Delete Groups: {groupsToDelete.Count}");

            if (groupsToDelete.Any())
            {
                await licenseGroupClient.Remove(groupsToDelete);
            }
        }

        public async Task ProcessUserGroups(List<LicenseUser> users)
        {
            Logger.Info(Environment.NewLine);
            Logger.Info("Processing group memberships.");
            var licenseUserClient = new LicenseUserClient();
            var licenseUserGroupClient = new LicenseUserGroupClient();

            Logger.Debug("Getting a list of local users and their groups.");
            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

            //Logger.Debug("Getting a list of remote users and their groups.");
            //IEnumerable<LicenseUser> remoteUsers = await licenseUserClient.GetAll();
            //var apiUsers = new List<LicenseUser>(remoteUsers);

            //Logger.Debug("Removing remote users that do not have any groups.");
            //foreach (LicenseUser apiUser in apiUsers)
            //{
            //    if (apiUser.Groups != null)
            //    {
            //        continue;
            //    }

            //    apiUser.Groups = new List<LicenseGroup>();
            //}

            foreach (LicenseGroup localGroup in localGroups)
            {
                var usersThatWereMembers = await licenseUserClient.GetAll(new ODataExpression<LicenseUser>(u => u.Groups.Any(g => g.Id == localGroup.Id)));
              //  List<LicenseUser> usersThatWereMembers = apiUsers.Where(u => u.Groups.Any(g => g.Id == localGroup.Id)).ToList();
                List<LicenseUser> usersThatAreMembers = users.Where(u => u.Groups.Any(g => g.Id == localGroup.Id)).ToList();

                List<LicenseUser> usersToBeAdded = GetUsersOrGroupsToCreate(usersThatWereMembers, usersThatAreMembers).ToList();

                Logger.Debug($"Processing: {localGroup.Name}");
                if (usersToBeAdded.Count > 0)
                {
                    Logger.Debug("Adding users");
                    await licenseUserGroupClient.Add(usersToBeAdded, localGroup);
                }

                List<LicenseUser> usersToBeRemoved = GetUsersOrGroupsToDelete(usersThatAreMembers, usersThatWereMembers.ToList()).ToList();

                if (usersToBeRemoved.Any())
                {
                    Logger.Debug("Removing users");
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
            var userManager = new UserManager();

            List<LicenseUser> localUsers = userManager.GetUsersAndGroups();
            Logger.Info($"Local users found: {localUsers.Count}");
            if (localUsers.Count == 0)
            {
                return localUsers;
            }

            IEnumerable<LicenseUser> remoteUsers = await licenseUserClient.GetAll(new ODataExpression<LicenseUser>(lu => lu.ManagedSupportId == uploadId));
            var apiUsers = new List<LicenseUser>(remoteUsers);

            List<LicenseUser> newUsers = GetUsersOrGroupsToCreate(localUsers, apiUsers, uploadId);
            Logger.Info($"New Users: {newUsers.Count}");

            if (newUsers.Any())
            {
                await licenseUserClient.Add(newUsers);
            }

            List<LicenseUser> usersToUpdate = GetUsersOrGroupsToUpdate<LicenseUser, LicenseUserCompareLogic>(localUsers, apiUsers).ToList();
            Logger.Info($"Update Users: {usersToUpdate.Count}");

            if (usersToUpdate.Any())
            {
                await licenseUserClient.Update(usersToUpdate);
            }

            /*
             * We need users that are not already deleted. Otherwise we will be deleting users that are already deleted,
             * wasting api calls. The reason for the new list is we need to return the original full list for other operations.
             */
            IEnumerable<LicenseUser> filteredUsers = await licenseUserClient.GetAll(new ODataExpression<LicenseUser>(lu => lu.ManagedSupportId == uploadId && !lu.IsDeleted));
            List<LicenseUser> usersToDelete = GetUsersOrGroupsToDelete(localUsers, filteredUsers.ToList());
            Logger.Info($"Delete Users: {usersToDelete.Count}");

            if (usersToDelete.Any())
            {
                await licenseUserClient.Remove(usersToDelete);
            }

            return localUsers;
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
                    ClientVersion = SettingManagerHelper.ClientVersion,
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
            if (users.Count == 0)
            {
                return;
            }

            await ProcessGroups(users);

            await ProcessUserGroups(users);

            await CallIn(managedSupportId);
        }
    }
}