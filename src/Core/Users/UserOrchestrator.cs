namespace LMS.Users
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Timing;
    using Castle.Core.Logging;
    using Common.Extensions;
    using Common.Helpers;
    using Compare;
    using Core.Configuration;
    using Core.OData;
    using KellermanSoftware.CompareNetObjects;
    using Managers;
    using Models;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserOrchestrator : ITransientDependency
    {

        private readonly PortalClient _portalClient;
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly ISettingManager _settingManager;
        private readonly IUserManager _userManager;
        private readonly IGroupManager _groupManager;

        public ILogger Logger { get; set; }

        public UserOrchestrator(PortalClient portalClient, IActiveDirectoryManager activeDirectoryManager, ISettingManager settingManager, IUserManager userManager, IGroupManager groupManager)
        {
            Logger = NullLogger.Instance;
            _portalClient = portalClient;
            _activeDirectoryManager = activeDirectoryManager;
            _settingManager = settingManager;
            _userManager = userManager;
            _groupManager = groupManager;
        }

        protected List<LicenseUserSummary> AddUsersToGroup(LicenseGroup group, List<LicenseUser> localUsers)
        {
            var remoteGroup = GetRemoteGroup(group.Id);

            List<LicenseUser> remoteUsers = _portalClient.ListAllUsersByGroupId(group.Id);

            List<LicenseUser> newMembers = localUsers.Where(u => remoteUsers.All(ru => ru.Id != u.Id)).ToList();
            if (!newMembers.Any())
            {
                Logger.Debug($"Group: {group.Name} - No new members");
                return new List<LicenseUserSummary>();
            }
            var usersAdded = new ConcurrentBag<LicenseUserSummary>();

            Parallel.ForEach(newMembers, newMember =>
            {
                _portalClient.Container.AttachTo("LicenseUsers", newMember);
                _portalClient.AddGroupToUser(newMember, remoteGroup);
                usersAdded.Add(new LicenseUserSummary { Id = newMember.Id, DisplayName = newMember.DisplayName, Status = LicenseUserGroupStatus.Added });
            });

            Logger.Debug($"Group: {group.Id}  Added: {usersAdded.Count}");
            return usersAdded.ToList();
        }

        public List<LicenseGroupSummary> DeleteGroups(List<LicenseGroup> localGroups)
        {
            List<LicenseGroupSummary> apiGroups = _portalClient.ListAllActiveGroupIds();

            apiGroups.RemoveAll(a => localGroups.Select(g => g.Id).Contains(a.Id));

            Parallel.ForEach(apiGroups, apiGroup => _portalClient.DeleteGroup(apiGroup.Id));

            return apiGroups;
        }

        public List<LicenseUserSummary> DeleteUsers(List<LicenseUser> localUsers)
        {
            List<LicenseUserSummary> apiUsers = _portalClient.ListAllActiveUserIds();

            apiUsers.RemoveAll(a => localUsers.Select(u => u.Id).Contains(a.Id));

            Parallel.ForEach(apiUsers, apiUser => _portalClient.DeleteUser(apiUser.Id));

            return apiUsers;
        }

        protected void PrintGroupMembershipSummary(LicenseUserGroupSummary summary, List<LicenseUserSummary> usersAdded, List<LicenseUserSummary> usersRemoved)
        {
            Logger.Info($"*** Group: {summary.Name}" + $"{(Logger.IsDebugEnabled ? "  Identifier: " + summary.Id : string.Empty)} ***");
            List<LicenseUserSummary> allUsers = usersAdded.Concat(usersRemoved).OrderBy(u => u.DisplayName).ToList();
            if (!allUsers.Any())
            {
                Logger.Info("No changes made.");
                return;
            }

            foreach (LicenseUserSummary sortedUser in allUsers)
            {
                bool isAdded = sortedUser.Status == LicenseUserGroupStatus.Added;
                string message = $"{(isAdded ? "+" : "-")} {sortedUser.DisplayName}  {(Logger.IsDebugEnabled ? "Identifier: " + sortedUser.Id : string.Empty)}";
                Logger.Info(message);
            }
        }

        public void ProcessCallIn(ManagedSupport managedSupport)
        {
            managedSupport.CheckInTime = new DateTimeOffset(Clock.Now);
            managedSupport.ClientVersion = SettingManagerHelper.Instance.ClientVersion;
            managedSupport.Hostname = Environment.MachineName;
            managedSupport.Status = CallInStatus.CalledIn;
            managedSupport.UploadId = _portalClient.GenerateUploadId();

            _portalClient.UpdateManagedSupport(managedSupport);
            _portalClient.SaveChanges();
        }

        public void ProcessGroups(ManagedSupport managedSupport)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUPS BEGIN");
            Logger.Info("Collecting information from Active Directory.");

            var groups = _activeDirectoryManager.GetGroups();
            var remoteGroups = _portalClient.ListAllActiveGroupIds();
            List<Guid> localGroupIds = new List<Guid>();
            foreach (var group in groups)
            {
                localGroupIds.Add(group.Id);

                bool existingGroup = remoteGroups.Any(ru => ru.Id == group.Id);
                if (existingGroup)
                {
                    _groupManager.Update(group);
                    continue;
                }

                _groupManager.Add(group, managedSupport.TenantId);
            }

            var groupsToDelete = remoteGroups.Where(ru => localGroupIds.All(u => u != ru.Id));
            foreach (var group in groupsToDelete)
            {
                _groupManager.Delete(group.Id);
            }

            Logger.Debug("PROCESS GROUPS END");
        }

        public ManagedSupport ProcessUpload()
        {
            Logger.Debug("PROCCESS UPLOAD BEGIN");
            Logger.Debug("Getting the id of the upload");
            Guid deviceId = _settingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

            int managedSupportId = _portalClient.GetManagedSupportId(deviceId);

            // create a new upload
            if (managedSupportId == default(int))
            {
                Logger.Debug("Looks like this device hasn't called in before.");
                Logger.Debug("Generating a new upload id.");

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
                managedSupportId = _portalClient.GetManagedSupportId(deviceId);
            }

            Logger.Debug("PROCCESS UPLOAD END");
            return _portalClient.ListManagedSupportById(managedSupportId);
        }

        public void ProcessUserGroups(List<LicenseUser> adUsers, List<LicenseGroup> adGroups)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUP MEMBERSHIP BEGIN");

            Logger.Info("Synchronizing Active Directory group memberships with the api...This might take some time.");

            List<LicenseGroup> groups = adGroups.OrderBy(g => g.Name).ToList();

            foreach (LicenseGroup adGroup in groups)
            {
                Logger.Debug($"Processing Active Directory group: {adGroup.Name} - {adGroup.Id}");

                var userGroup = new LicenseUserGroupSummary { Id = adGroup.Id, Name = adGroup.Name };

                List<LicenseUser> localUsers = adUsers.Where(u => u.Groups.Any(g => g.Id == adGroup.Id)).ToList();
                List<LicenseUserSummary> usersAdded = AddUsersToGroup(adGroup, localUsers);
                List<LicenseUserSummary> usersRemoved = RemoveUsersFromGroup(adGroup, localUsers);

                if (adGroup == groups.First())
                {
                    Console.WriteLine(Environment.NewLine);
                    Logger.Info("     User Group Membership Summary");
                    Logger.Info("----------------------------------------");
                }
                else
                {
                    Console.WriteLine(Environment.NewLine);
                }

                PrintGroupMembershipSummary(userGroup, usersAdded, usersRemoved);
            }

            Logger.Debug("PROCESS GROUP MEMBERSHIP END");
        }

        /// <summary>
        /// Decides whether a License User object should be Added, Updated or Deleted from the API.
        /// </summary>
        /// <param name="managedSupport"></param>
        public void ProcessUsers(ManagedSupport managedSupport)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS USERS BEGIN");
            Logger.Info("Collecting information from Active Directory.");

            var users = _activeDirectoryManager.GetUsers();
            var remoteUsers = _portalClient.ListAllActiveUserIds();
            List<Guid> localUserIds = new List<Guid>();
            foreach (var user in users)
            {
                localUserIds.Add(user.Id);

                bool existingUser = remoteUsers.Any(ru => ru.Id == user.Id);
                if (existingUser)
                {
                    _userManager.Update(user);
                    continue;
                }

                _userManager.Add(user, managedSupport.Id, managedSupport.TenantId);
            }

            var usersToDelete = remoteUsers.Where(ru => localUserIds.All(u => u != ru.Id));
            foreach (var user in usersToDelete)
            {
                _userManager.Delete(user.Id);
            }

            Logger.Debug("PROCESS USERS END");
        }

        protected LicenseGroup GetRemoteGroup(Guid groupId)
        {
            LicenseGroup remoteGroup = _portalClient.ListGroupById(groupId);
            _portalClient.Container.AttachTo("LicenseGroups", remoteGroup);

            return remoteGroup;
        }

        protected List<LicenseUserSummary> RemoveUsersFromGroup(LicenseGroup group, List<LicenseUser> localUsers)
        {
            var remoteGroup = GetRemoteGroup(group.Id);

            List<LicenseUser> remoteUsers = _portalClient.ListAllUsersByGroupId(group.Id);

            List<LicenseUser> usersToRemove = remoteUsers.Except(localUsers, new LicenseUserComparer()).ToList();

            if (!usersToRemove.Any())
            {
                Logger.Debug($"Group: {group.Name} - No old members");
                return new List<LicenseUserSummary>();
            }
            var usersRemoved = new ConcurrentBag<LicenseUserSummary>();

            Parallel.ForEach(usersToRemove, oldMember =>
            {
                _portalClient.Container.AttachTo("LicenseUsers", oldMember);
                _portalClient.DeleteGroupFromUser(oldMember, remoteGroup);
                usersRemoved.Add(new LicenseUserSummary { Id = oldMember.Id, DisplayName = oldMember.DisplayName, Status = LicenseUserGroupStatus.Removed });
            });

            Logger.Debug($"Group: {group.Id}  Removed: {usersToRemove.Count}");
            return usersRemoved.ToList();
        }

        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info("Stopwatch started!");
            Logger.Info("Processing the upload information");
            ManagedSupport managedSupport = ProcessUpload();

            ProcessUsers(managedSupport);
            ProcessGroups(managedSupport);

            //List<LicenseUser> adUsersAndGroups = ProcessUsers(managedSupport);
            //if (!adUsersAndGroups.Any())
            //{
            //    return;
            //}

            //List<LicenseGroup> adGroups = ProcessGroups(adUsersAndGroups);

            //ProcessUserGroups(adUsersAndGroups, adGroups);

            //ProcessCallIn(managedSupport);

            //stopWatch.Stop();
            //Console.WriteLine(Environment.NewLine);
            //Logger.Info($"Time elapsed: {stopWatch.Elapsed:hh\\:mm\\:ss}");
        }
    }
}