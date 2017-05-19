namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using Abp.Domain.Services;
    using Common;
    using Common.Extensions;
    using Models;

    public class UserManager : DomainService, IUserManager
    {
        public List<LicenseUser> GetUsersAndGroups()
        {
            return AllUsers();
        }

        /// <summary>
        ///     Returns a list of all the users from Active Directory.
        /// </summary>
        /// <returns></returns>
        private List<LicenseUser> AllUsers()
        {
            var localUsers = new List<LicenseUser>();

            try
            {
                var context = new PrincipalContext(ContextType.Domain);

                // get the groups first
                var groupSearch = new PrincipalSearcher(new GroupPrincipal(context));

                var groups = groupSearch.FindAll()
                    .Where(g => g.Guid != null)
                    .Cast<GroupPrincipal>()
                    .Where(g => g.IsSecurityGroup != null && (bool) g.IsSecurityGroup)
                    .Select(g => new
                    {
                        Id = g.Guid,
                        g.Name,
                        Members = g.Members.Select(m => m.Guid).ToList(),
                        WhenCreated = DateTime.Parse(g.GetProperty("whenCreated"))
                    }).ToList();

                // then process the users
                var userSearch = new PrincipalSearcher(new UserPrincipal(context));

                List<UserPrincipal> users = userSearch.FindAll()
                    .Where(u => u.Guid != null)
                    .Cast<UserPrincipal>()
                    .ToList();

                foreach (UserPrincipal user in users)
                {
                    var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;

                    localUsers.Add(new LicenseUser
                    {
                        DisplayName = user.DisplayName,
                        Email = user.EmailAddress,
                        Enabled = !dirEntry.IsAccountDisabled(),
                        FirstName = user.GivenName,
                        Groups = groups.Where(g => g.Members.Any(m => m == user.Guid))
                            .Select(g => new LicenseGroup
                            {
                                Id = Guid.Parse(g.Id.ToString()),
                                Name = g.Name,
                                WhenCreated = g.WhenCreated
                            }).ToList(),
                        Id = Guid.Parse(user.Guid.ToString()),
                        Surname = user.Surname,
                        WhenCreated = DateTime.Parse(user.GetProperty("whenCreated"))
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex.ToString());
                throw;
            }

            return localUsers;
        }
    }
}