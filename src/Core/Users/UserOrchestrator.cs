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
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserOrchestrator : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _disposed;
        protected PortalClient PortalClient = new PortalClient();
        protected UserManager UserManager = new UserManager();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected List<LicenseUserSummary> AddUsersToGroup(LicenseGroup group, List<LicenseUser> localUsers)
        {
            var remoteGroup = GetRemoteGroup(group.Id);

            List<LicenseUser> remoteUsers = PortalClient.ListAllUsersByGroupId(group.Id);

            List<LicenseUser> newMembers = localUsers.Where(u => remoteUsers.All(ru => ru.Id != u.Id)).ToList();
            if (!newMembers.Any())
            {
                Logger.Debug($"Group: {group.Name} - No new members");
                return new List<LicenseUserSummary>();
            }
            var usersAdded = new ConcurrentBag<LicenseUserSummary>();

            Parallel.ForEach(newMembers, newMember =>
            {
                PortalClient.Container.AttachTo("LicenseUsers", newMember);
                PortalClient.AddGroupToUser(newMember, remoteGroup);
                usersAdded.Add(new LicenseUserSummary {Id = newMember.Id, DisplayName = newMember.DisplayName, Status = LicenseUserGroupStatus.Added});
            });

            Logger.Debug($"Group: {group.Id}  Added: {usersAdded.Count}");
            return usersAdded.ToList();
        }

        public List<LicenseGroupSummary> DeleteGroups(List<LicenseGroup> localGroups)
        {
            List<LicenseGroupSummary> apiGroups = PortalClient.ListAllActiveGroupIds();

            apiGroups.RemoveAll(a => localGroups.Select(g => g.Id).Contains(a.Id));

            Parallel.ForEach(apiGroups, apiGroup => PortalClient.DeleteGroup(apiGroup.Id));

            return apiGroups;
        }

        public List<LicenseUserSummary> DeleteUsers(List<LicenseUser> localUsers)
        {
            List<LicenseUserSummary> apiUsers = PortalClient.ListAllActiveUserIds();

            apiUsers.RemoveAll(a => localUsers.Select(u => u.Id).Contains(a.Id));

            Parallel.ForEach(apiUsers, apiUser => PortalClient.DeleteUser(apiUser.Id));

            return apiUsers;
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

        protected void PrintGroupMembershipSummary(LicenseUserGroupSummary summary, List<LicenseUserSummary> usersAdded, List<LicenseUserSummary> usersRemoved)
        {
            Logger.Info($"*** Group: {summary.Name}" +  $"{(Logger.IsDebugEnabled ? "  Identifier: " + summary.Id : string.Empty)} ***");
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
            managedSupport.UploadId = PortalClient.GenerateUploadId();

            PortalClient.UpdateManagedSupport(managedSupport);
            PortalClient.SaveChanges();
        }

        public List<LicenseGroup> ProcessGroups(List<LicenseUser> users)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUPS BEGIN");
            Logger.Info("Collecting information from Active Directory.");

            List<LicenseGroup> adGroups = users.SelectMany(u => u.Groups).Distinct(new LicenseGroupComparer()).ToList();

            Logger.Info("Synchronizing Active Directory groups with the api...This might take some time.");
            var compareLogic = new LicenseGroupCompareLogic();

            var groupsAdded = new ConcurrentBag<LicenseGroupSummary>();
            var groupsUpdated = new ConcurrentBag<LicenseGroupSummary>();

            List<LicenseGroup> remoteGroups = PortalClient.ListAllGroups();

            Parallel.ForEach(adGroups, adGroup =>
            {
                Logger.Debug($"Processing Active Directory group: {adGroup.Name} - {adGroup.Id}");

                LicenseGroup remoteGroup = remoteGroups.FirstOrDefault(g => g.Id == adGroup.Id);
                if (remoteGroup == null)
                {
                    PortalClient.AddGroup(adGroup);
                    groupsAdded.Add(new LicenseGroupSummary {Id = adGroup.Id, Name = adGroup.Name});
                    return;
                }

                ComparisonResult result = compareLogic.Compare(adGroup, remoteGroup);
                if (result.AreEqual)
                {
                    return;
                }

                Logger.Debug($"Updating group: {adGroup.Name} - {adGroup.Id}  Difference: {result.DifferencesString}");
                Logger.Debug($"Difference: {result.DifferencesString}");
                PortalClient.UpdateGroup(adGroup);
                groupsUpdated.Add(new LicenseGroupSummary {Id = adGroup.Id, Name = adGroup.Name});
            });

            PortalClient.SaveChanges(true);

            List<LicenseGroupSummary> groupsDeleted = DeleteGroups(adGroups);

            PortalClient.SaveChanges(true);

            Console.WriteLine(Environment.NewLine);
            Logger.Info("     Group Summary");
            Logger.Info("----------------------------------------");
            Logger.Info($"Created: {groupsAdded.Count}");
            foreach (LicenseGroupSummary group in groupsAdded)
            {
                Logger.Info($"+ {group.Name}" + (Logger.IsDebugEnabled ? $"  Identifier: {group.Id}" : string.Empty));
            }
            Logger.Info($"Updated: {groupsUpdated.Count}");
            foreach (LicenseGroupSummary group in groupsUpdated)
            {
                Logger.Info($"^ {group.Name}" + (Logger.IsDebugEnabled ? $"  Identifier: {group.Id}" : string.Empty));
            }
            Logger.Info($"Deleted: {groupsDeleted.Count}");
            foreach (LicenseGroupSummary group in groupsDeleted)
            {
                Logger.Info($"- {group.Name}" + (Logger.IsDebugEnabled ? $"  Identifier: {group.Id}" : string.Empty));
            }

            Logger.Debug("PROCESS GROUPS END");
            return adGroups;
        }

        public ManagedSupport ProcessUpload()
        {
            Logger.Debug("PROCCESS UPLOAD BEGIN");
            Logger.Debug("Getting the id of the upload");
            Guid deviceId = SettingManagerHelper.Instance.DeviceId;

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
                    ClientVersion = SettingManagerHelper.Instance.ClientVersion,
                    DeviceId = deviceId,
                    Hostname = Environment.MachineName,
                    IsActive = true,
                    Status = CallInStatus.NotCalledIn,
                    UploadId = uploadId
                };

                PortalClient.AddManagedSupport(ms);
                PortalClient.SaveChanges();
                managedSupportId = PortalClient.GetManagedSupportId(deviceId);
            }

            Logger.Debug("PROCCESS UPLOAD END");
            return PortalClient.ListManagedSupportById(managedSupportId);
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

                var userGroup = new LicenseUserGroupSummary {Id = adGroup.Id, Name = adGroup.Name};

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

            Logger.Info("Synchronizing Active Directory users with the api...This might take some time.");
            var userCompareLogic = new LicenseUserCompareLogic();

            var usersAdded = new ConcurrentBag<LicenseUserSummary>();
            var usersUpdated = new ConcurrentBag<LicenseUserSummary>();

            List<LicenseUser> remoteUsers = PortalClient.ListAllUsers();

            Parallel.ForEach(adUsersAndGroups, adUser =>
            {
                Logger.Debug($"Processing Active Directory user: {adUser.DisplayName} - {adUser.Id}");
                adUser.ManagedSupportId = managedSupport.Id;
                adUser.TenantId = managedSupport.TenantId;
                Parallel.ForEach(adUser.Groups, group => group.TenantId = managedSupport.TenantId);

                LicenseUser remoteUser = remoteUsers.FirstOrDefault(u => u.Id == adUser.Id);
                if (remoteUser == null)
                {
                    PortalClient.AddUser(adUser);
                    usersAdded.Add(new LicenseUserSummary {Id = adUser.Id, DisplayName = adUser.DisplayName});
                    return;
                }

                ComparisonResult result = userCompareLogic.Compare(adUser, remoteUser);
                if (result.AreEqual)
                {
                    return;
                }

                Logger.Debug($"Updating user: {adUser.DisplayName} - {adUser.Id}  Difference: {result.DifferencesString}");
                PortalClient.UpdateUser(adUser);
                usersUpdated.Add(new LicenseUserSummary {Id = adUser.Id, DisplayName = adUser.DisplayName});
            });

            PortalClient.SaveChanges(true);

            List<LicenseUserSummary> usersDeleted = DeleteUsers(adUsersAndGroups);

            Console.WriteLine(Environment.NewLine);
            Logger.Info("     User Summary");
            Logger.Info("----------------------------------------");
            Logger.Info($"Created: {usersAdded.Count}");
            foreach (LicenseUserSummary user in usersAdded)
            {
                Logger.Info($"+ {user.DisplayName}" + (Logger.IsDebugEnabled ? $"  Identifier: {user.Id}" : string.Empty));
            }

            Logger.Info($"Updated: {usersUpdated.Count}");
            foreach (LicenseUserSummary user in usersUpdated)
            {
                Logger.Info($"^ {user.DisplayName}" + (Logger.IsDebugEnabled ? $"  Identifier: {user.Id}" : string.Empty));
            }

            Logger.Info($"Deleted: {usersDeleted.Count}");
            foreach (LicenseUserSummary user in usersDeleted)
            {
                Logger.Info($"- {user.DisplayName}" + (Logger.IsDebugEnabled ? $"  Identifier: {user.Id}" : string.Empty));
            }

            Logger.Debug("PROCESS USERS END");
            return adUsersAndGroups;
        }

        protected LicenseGroup GetRemoteGroup(Guid groupId)
        {
            // we want to track changes now.
            PortalClient = new PortalClient();

            LicenseGroup remoteGroup = PortalClient.ListGroupById(groupId);
            PortalClient.Container.AttachTo("LicenseGroups", remoteGroup);

            return remoteGroup;
        }

        protected List<LicenseUserSummary> RemoveUsersFromGroup(LicenseGroup group, List<LicenseUser> localUsers)
        {
            var remoteGroup = GetRemoteGroup(group.Id);

            List<LicenseUser> remoteUsers = PortalClient.ListAllUsersByGroupId(group.Id);

            List<LicenseUser> usersToRemove = remoteUsers.Except(localUsers, new LicenseUserComparer()).ToList();

            if (!usersToRemove.Any())
            {
                Logger.Debug($"Group: {group.Name} - No old members");
                return new List<LicenseUserSummary>();
            }
            var usersRemoved = new ConcurrentBag<LicenseUserSummary>();

            Parallel.ForEach(usersToRemove, oldMember =>
            {
                PortalClient.Container.AttachTo("LicenseUsers", oldMember);
                PortalClient.DeleteGroupFromUser(oldMember, remoteGroup);
                usersRemoved.Add(new LicenseUserSummary {Id = oldMember.Id, DisplayName = oldMember.DisplayName, Status = LicenseUserGroupStatus.Removed});
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