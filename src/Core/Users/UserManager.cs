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
            try
            {
                var users = new List<LicenseUser>();
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var search = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        IEnumerable<UserPrincipal> allUsers = search.FindAll().Cast<UserPrincipal>();

                        foreach (UserPrincipal user in allUsers)
                        {
                            try
                            {
                                var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;

                                var localUser = new LicenseUser
                                {
                                    DisplayName = user.DisplayName,
                                    Email = user.EmailAddress,
                                    Enabled = !dirEntry.IsAccountDisabled(),
                                    FirstName = user.GivenName,
                                    Groups = user.GetAuthorizationGroups().Where(g => g is GroupPrincipal && g.Guid != null).Select(g => new LicenseGroup
                                    {
                                        Id = Guid.Parse(g.Guid.ToString()),
                                        Name = g.Name,
                                        WhenCreated = DateTime.Parse(g.GetProperty("whenCreated"))
                                    }).ToList(),
                                    Id = Guid.Parse(user.Guid.ToString()),
                                    Surname = user.Surname,
                                    WhenCreated = DateTime.Parse(user.GetProperty("whenCreated"))
                                };

                                users.Add(localUser);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"There was a problem processing {user.Name}.");
                                Logger.Error(ex.Message);
                                Logger.Debug($"Could not convert the following UserPrinciple into a User object: {user.Dump()}");
                                Logger.Debug(ex.ToString());
                            }
                        }

                        return users;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString());
                throw;
            }
        }
    }
}