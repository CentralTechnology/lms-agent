namespace LMS.Users
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Abp.Configuration;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core.OData;
    using Dto;
    using Managers;
    using Models;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserOrchestrator : ITransientDependency
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly IGroupManager _groupManager;
        private readonly IManagedSupportManager _managedSupportManager;    
        private readonly PortalClient _portalClient;
        private readonly ISettingManager _settingManager;
        private readonly IUserGroupManager _userGroupManager;
        private readonly IUserManager _userManager;

        public UserOrchestrator(
            PortalClient portalClient,
            IActiveDirectoryManager activeDirectoryManager,
            ISettingManager settingManager,
            IUserManager userManager,
            IGroupManager groupManager,
            IManagedSupportManager managedSupportManager,
            IUserGroupManager userGroupManager
        )
        {
            Logger = NullLogger.Instance;
            _portalClient = portalClient;
            _activeDirectoryManager = activeDirectoryManager;
            _settingManager = settingManager;
            _userManager = userManager;
            _groupManager = groupManager;
            _managedSupportManager = managedSupportManager;
            _userGroupManager = userGroupManager;
        }

        public ILogger Logger { get; set; }

        public void ProcessGroups(ManagedSupport managedSupport)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUPS BEGIN");
            Logger.Info("Collecting information from Active Directory.");

            IEnumerable<LicenseGroupDto> groups = _activeDirectoryManager.GetGroups();
            List<LicenseGroupSummary> remoteGroups = _portalClient.ListAllActiveGroupIds();
            var localGroupIds = new List<Guid>();
            foreach (LicenseGroupDto group in groups)
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

            IEnumerable<LicenseGroupSummary> groupsToDelete = remoteGroups.Where(ru => localGroupIds.All(u => u != ru.Id));
            foreach (LicenseGroupSummary group in groupsToDelete)
            {
                _groupManager.Delete(group.Id);
            }

            Logger.Debug("PROCESS GROUPS END");
        }

        public void ProcessUserGroups()
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS GROUP MEMBERSHIP BEGIN");

            Logger.Info("Synchronizing Active Directory group memberships with the api...This might take some time.");

            IEnumerable<LicenseGroupDto> groups = _activeDirectoryManager.GetGroups();
            foreach (LicenseGroupDto group in groups)
            {
                LicenseGroupUsersDto localMembers = _activeDirectoryManager.GetGroupMembers(group.Id);

                _userGroupManager.AddUsersToGroup(localMembers);
                _userGroupManager.DeleteUsersFromGroup(localMembers);
            }

            Logger.Debug("PROCESS GROUP MEMBERSHIP END");
        }

        /// <summary>
        ///     Decides whether a License User object should be Added, Updated or Deleted from the API.
        /// </summary>
        /// <param name="managedSupport"></param>
        public void ProcessUsers(ManagedSupport managedSupport)
        {
            Console.WriteLine(Environment.NewLine);
            Logger.Debug("PROCESS USERS BEGIN");
            Logger.Info("Collecting information from Active Directory.");

            IEnumerable<LicenseUserDto> users = _activeDirectoryManager.GetUsers();
            List<LicenseUserSummary> remoteUsers = _portalClient.ListAllActiveUserIds();
            var localUserIds = new List<Guid>();
            foreach (LicenseUserDto user in users)
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

            IEnumerable<LicenseUserSummary> usersToDelete = remoteUsers.Where(ru => localUserIds.All(u => u != ru.Id));
            foreach (LicenseUserSummary user in usersToDelete)
            {
                _userManager.Delete(user.Id);
            }

            Logger.Debug("PROCESS USERS END");
        }

        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Logger.Info("Stopwatch started!");
            Logger.Info("Processing the upload information");
            ManagedSupport managedSupport = _managedSupportManager.Get() ?? _managedSupportManager.Add();
            _portalClient.Detach(managedSupport);

            ProcessUsers(managedSupport);
            ProcessGroups(managedSupport);
            ProcessUserGroups();

            // let the api know we have completed the task
            _managedSupportManager.Update(managedSupport);

            stopWatch.Stop();
            Console.WriteLine(Environment.NewLine);
            Logger.Info($"Time elapsed: {stopWatch.Elapsed:hh\\:mm\\:ss}");
        }
    }
}