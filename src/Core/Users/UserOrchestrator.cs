namespace Core.Users
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Timing;
    using Common.Helpers;
    using Compare;
    using KellermanSoftware.CompareNetObjects;
    using Managers;
    using Models;
    using NLog;
    using OData;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserOrchestrator : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        bool _disposed;
        protected PortalClient PortalClient = new PortalClient();
        protected UserManager UserManager = new UserManager();

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
                    UserManager = null;
                }
            }

            _disposed = true;
        }

        public void ProcessCallIn(ManagedSupport managedSupport)
        {
            PortalClient.UpdateManagedSupport(managedSupport.Id, new ManagedSupportUpdateModel
            {
                CheckInTime = new DateTimeOffset(Clock.Now),
                ClientVersion = SettingManagerHelper.ClientVersion,
                Hostname = Environment.MachineName,
                Status = Portal.Common.Enums.CallInStatus.CalledIn,
                UploadId = PortalClient.GenerateUploadId()
            });
        }

        public List<LicenseGroup> ProcessGroups(List<LicenseUser> users)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUPS BEGIN");

            List<LicenseGroup> adGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

            int deletedGroups = 0;

            Logger.Info("Synchronizing Active Directory groups with the api...This might take some time.");
            var compareLogic = new LicenseGroupCompareLogic();

            var groupsToAdd = new ConcurrentBag<LicenseGroup>();
            var groupsToUpdate = new ConcurrentBag<LicenseGroupUpdateModel>();

            Parallel.ForEach(adGroups, adGroup =>
            {
                Logger.Debug($"Processing Active Directory group: {adGroup.Name} - {adGroup.Id}");

                LicenseGroup remoteGroup = PortalClient.ListGroupById(adGroup.Id);
                if (remoteGroup == null)
                {
                    Logger.Debug($"Creating new group: {adGroup.Name} - {adGroup.Id}");
                    groupsToAdd.Add(adGroup);
                    return;
                }

                ComparisonResult result = compareLogic.Compare(adGroup, remoteGroup);
                if (result.AreEqual)
                {
                    Logger.Debug("No update required");
                    return;
                }

                Logger.Debug($"Updating existing group: {adGroup.Name} - {adGroup.Id}");
                Logger.Debug($"Difference: {result.DifferencesString}");
                groupsToUpdate.Add(new LicenseGroupUpdateModel
                {
                    Id = adGroup.Id,
                    Name = adGroup.Name,
                    WhenCreated = adGroup.WhenCreated
                });
            });

            PortalClient.AddGroup(groupsToAdd);
            PortalClient.UpdateGroup(groupsToUpdate);

            List<Guid> apiGroupIds = PortalClient.ListAllActiveGroupIds();

            apiGroupIds.RemoveAll(a => adGroups.Select(u => u.Id).Contains(a));

            foreach (Guid apiGroupId in apiGroupIds)
            {
                Logger.Debug($"Deleting group: {apiGroupId}");
                PortalClient.DeleteGroup(apiGroupId);

                deletedGroups++;
            }

            Console.WriteLine(Environment.NewLine);
            Logger.Info("     Group Summary");
            Logger.Info("----------------------------------------");
            Logger.Info($"Created: {groupsToAdd.Count}");
            Logger.Info($"Updated: {groupsToUpdate.Count}");
            Logger.Info($"Deleted: {deletedGroups}");

            Logger.Debug("PROCESS GROUPS END");
            return adGroups;
        }

        public ManagedSupport ProcessUpload()
        {
            Logger.Debug("PROCCESS UPLOAD BEGIN");
            Logger.Debug("Getting the id of the upload");
            Guid deviceId = SettingManagerHelper.DeviceId;

            int managedSupportId = PortalClient.GetManagedSupportId(deviceId);

            // create a new upload
            if (managedSupportId == default(int))
            {
                Logger.Debug("Looks like this device hasn't called in before.");
                Logger.Debug("Generating a new upload id.");

                int uploadId = PortalClient.GenerateUploadId();

                var ms = new ManagedSupport
                {
                    CheckInTime = Clock.Now,
                    ClientVersion = SettingManagerHelper.ClientVersion,
                    DeviceId = deviceId,
                    Hostname = Environment.MachineName,
                    IsActive = true,
                    Status = Portal.Common.Enums.CallInStatus.NotCalledIn,
                    UploadId = uploadId
                };

                PortalClient.AddManagedSupport(ms);
                managedSupportId = PortalClient.GetManagedSupportId(deviceId);
            }

            Logger.Debug("PROCCESS UPLOAD END");
            return PortalClient.ListManagedSupportById(managedSupportId);
        }

        public void ProcessUserGroups(List<LicenseUser> adUsers, List<LicenseGroup> adGroups)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUP MEMBERSHIP BEGIN");

            var usersAdded = new ConcurrentBag<LicenseUser>();
            var usersRemoved = new ConcurrentBag<LicenseUser>();

            Logger.Info("Synchronizing Active Directory group memberships with the api...This might take some time.");

            foreach (var adGroup in adGroups)
            {
                Logger.Debug($"Processing Active Directory group: {adGroup.Name} - {adGroup.Id}");

                List<LicenseUser> apiMembers = PortalClient.ListAllUsersByGroupId(adGroup.Id);
                List<LicenseUser> adMembers = adUsers.Where(u => u.Groups.Any(g => g.Id == adGroup.Id)).ToList();

                IEnumerable<LicenseUser> usersToAdd = adMembers.Where(ad => apiMembers.All(ap => ap.Id != ad.Id));
                LicenseGroup group = PortalClient.ListGroupById(adGroup.Id);
                foreach (LicenseUser userToAdd in usersToAdd)
                {
                    Logger.Debug($"Adding user: {userToAdd.Id} To group:{adGroup.Name}");
                    PortalClient.AddGroupToUser(userToAdd, group);

                    usersAdded.Add(userToAdd);
                }

                var usersToRemove = new List<LicenseUser>(apiMembers);
                usersToRemove.RemoveAll(ap => adMembers.Select(ad => ad.Id).Contains(ap.Id));
                foreach (LicenseUser userToRemove in usersToRemove)
                {
                    Logger.Debug($"Removing user: {userToRemove.Id} From group:{adGroup.Name}");
                    PortalClient.DeleteGroupFromUser(userToRemove, group);

                    usersRemoved.Add(userToRemove);
                }
            }

            Console.WriteLine(Environment.NewLine);
            Logger.Info("     User Group Membership Summary");
            Logger.Info("----------------------------------------");
            Logger.Info($"Users Added to Groups: {usersAdded.Count}");
            Logger.Info($"Users Removed from Groups: {usersRemoved.Count}");

            Logger.Debug("PROCESS GROUP MEMBERSHIP END");
        }

        public List<LicenseUser> ProcessUsers(ManagedSupport managedSupport)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS USERS BEGIN");
            Logger.Info("Collecting information from Active Directory.");

            List<LicenseUser> adUsersAndGroups = UserManager.AllUsers().ToList();
            if (!adUsersAndGroups.Any())
            {
                Logger.Warn("No Active Directory users could be found. Processing has been cancelled.");
                return new List<LicenseUser>();
            }

            int deletedUsers = 0;

            Logger.Info("Synchronizing Active Directory users with the api...This might take some time.");
            var userCompareLogic = new LicenseUserCompareLogic();

            var usersToAdd = new ConcurrentBag<LicenseUser>();
            var usersToUpdate = new ConcurrentBag<LicenseUserUpdateModel>();

            Parallel.ForEach(adUsersAndGroups, adUser =>
            {
                Logger.Debug($"Processing Active Directory user: {adUser.DisplayName} - {adUser.Id}");
                adUser.ManagedSupportId = managedSupport.Id;
                adUser.TenantId = managedSupport.TenantId;
                Parallel.ForEach(adUser.Groups, g => g.TenantId = managedSupport.TenantId);

                LicenseUser remoteUser = PortalClient.ListUserById(adUser.Id);
                if (remoteUser == null)
                {
                    Logger.Debug($"Creating new user: {adUser.DisplayName} - {adUser.Id}");
                    usersToAdd.Add(adUser);
                    return;
                }

                ComparisonResult result = userCompareLogic.Compare(adUser, remoteUser);
                if (result.AreEqual)
                {
                    Logger.Debug("No update required");
                    return;
                }

                Logger.Debug($"Updating existing user: {adUser.DisplayName} - {adUser.Id}");
                Logger.Debug($"Difference: {result.DifferencesString}");
                usersToUpdate.Add(new LicenseUserUpdateModel
                {
                    DisplayName = adUser.DisplayName,
                    Email = adUser.Email,
                    Enabled = adUser.Enabled,
                    FirstName = adUser.FirstName,
                    Id = adUser.Id,
                    LastLoginDate = adUser.LastLoginDate,
                    SamAccountName = adUser.SamAccountName,
                    Surname = adUser.Surname,
                    WhenCreated = adUser.WhenCreated
                });
            });

            PortalClient.AddUser(usersToAdd);
            PortalClient.UpdateUser(usersToUpdate);

            List<Guid> apiUserIds = PortalClient.ListAllActiveUserIds();

            apiUserIds.RemoveAll(a => adUsersAndGroups.Select(u => u.Id).Contains(a));

            foreach (Guid apiUserId in apiUserIds)
            {
                Logger.Debug($"Deleting user: {apiUserId}");
                PortalClient.DeleteUser(apiUserId);

                deletedUsers++;
            }

            Console.WriteLine(Environment.NewLine);
            Logger.Info("     User Summary");
            Logger.Info("----------------------------------------");
            Logger.Info($"Created: {usersToAdd.Count}");
            Logger.Info($"Updated: {usersToUpdate.Count}");
            Logger.Info($"Deleted: {deletedUsers}");

            Logger.Debug("PROCESS USERS END");
            return adUsersAndGroups;
        }

        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info("Stopwatch started!");
            Logger.Info("Processing the upload information");
            ManagedSupport managedSupport = ProcessUpload();

            List<LicenseUser> adUsersAndGroups = ProcessUsers(managedSupport);
            if (!adUsersAndGroups.Any())
            {
                return;
            }

            List<LicenseGroup> adGroups = ProcessGroups(adUsersAndGroups);

            ProcessUserGroups(adUsersAndGroups, adGroups);

            ProcessCallIn(managedSupport);

            stopWatch.Stop();
            Console.WriteLine(Environment.NewLine);
            Logger.Info($"Time elapsed: {stopWatch.Elapsed:hh\\:mm\\:ss}");
        }
    }
}