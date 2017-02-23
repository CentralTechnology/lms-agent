namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using Common;
    using Core;
    using LicenseMonitoringSystem.Core.Common;
    using Settings;

    public class UserManager : LicenseMonitoringBase, IUserManager
    {
        public UserManager(SettingManager settingManager)
            : base(settingManager)
        {
        }

        public List<User> GetUsersAndGroups()
        {
            Logger.Info("Collecting user information");
            return AllUsers();
        }

        /// <summary>
        ///     Returns a list of all the users from Active Directory.
        /// </summary>
        /// <returns></returns>
        private List<User> AllUsers()
        {
            List<User> users = new List<User>();

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                using (var search = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    var allUsers = search.FindAll().Cast<UserPrincipal>().ToList();
                    Logger.DebugFormat("{0} users found", allUsers.Count);

                    users.AddRange(allUsers.Select(u => new User
                    {
                        DisplayName = u.DisplayName,
                        Email = u.EmailAddress,
                        Enabled = u.Enabled ?? false,
                        FirstName = u.GivenName,
                        Groups = u.GetAuthorizationGroups().Where(g => g is GroupPrincipal && g.Guid != null).Select(g => new UserGroup
                        {
                            Id = Guid.Parse(g.Guid.ToString()),
                            Name = g.Name,
                            UserId = Guid.Parse(u.Guid.ToString()),
                            WhenCreated = DateTime.Parse(g.GetProperty("whenCreated"))
                        }).ToList(),
                        Id = Guid.Parse(u.Guid.ToString()),
                        Surname = u.Surname,
                        WhenCreated = DateTime.Parse(u.GetProperty("whenCreated"))
                    }).ToList());
                }
            }

            return users;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private List<User> GetActiveUsers()
        {
            var activeUsers = AllUsers().Where(u => u.Enabled).ToList();

            Logger.DebugFormat("{0} active users found", activeUsers.Count);
            return activeUsers;
        }
    }
}