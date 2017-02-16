namespace LicenseMonitoringSystem.Core
{
    using System;
    using System.Linq;
    using System.Linq.Dynamic;
    using Abp;
    using Abp.Dependency;
    using Common;
    using Common.Client;
    using Newtonsoft.Json;
    using Settings;
    using Users;

    public class Orchestrator : LicenseMonitoringBase, ISingletonDependency
    { 
        private readonly IUserManager _userManager;
        private readonly IPortalClient _portalClient;
        public Orchestrator(
            IUserManager userManager, 
            IPortalClient portalClient,
            SettingManager settingManager) 
            : base(settingManager)
        {
            _userManager = userManager;
            _portalClient = portalClient;
        }

        public void Run(Monitor monitor)
        {
            switch (monitor)
            {
                case Monitor.Users:
                    Logger.Info("Monitoring Users");
                    Users();
                    break;
                default:
                    Logger.Error("No monitors selected. Please check the Settings.json file.");
                    Environment.Exit(1);
                    break;
            }
        }

        private void Users()
        {
            var localUsers = _userManager.GetUsersAndGroups();
            if (localUsers.Count == 0)
            {
                Logger.Error("Failed to retrieve any users from the local system.");
                return;
            }

            var remoteUsers = _portalClient.GetAllUsers();
            if (remoteUsers.Count == 0)
            {
                Logger.Error("Failed to retrieve any users from the API.");
                return;
            }

            // create new users
            var newUsers = localUsers.Where(l => remoteUsers.Any(r => l.Id != r.Id)).ToList();
            Logger.InfoFormat("Users that require creating: {0}", newUsers.Count);
            foreach (var user in newUsers)
            {
                _portalClient.CreateUser(user);
            }

            var updateUsers = localUsers.Where(l => newUsers.Any(n => l.Id == n.Id)).ToList();
            Logger.InfoFormat("Users that require updating: {0}", updateUsers.Count);
            foreach (var user in updateUsers)
            {
                _portalClient.UpdateUser(user);
            }

            var deleteUsers = remoteUsers.Where(r => localUsers.Any(l => r.Id != l.Id)).ToList();
            Logger.InfoFormat("Users that require deleting: {0}", deleteUsers.Count);
            foreach (var user in deleteUsers)
            {
                _portalClient.DeleteUser(user.Id);
            }
        }       
    }
}