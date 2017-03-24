namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Castle.Core.Logging;
    using Common;
    using Common.Extensions;
    using Models;
    using ShellProgressBar;

    public class UserManager : DomainService, IUserManager
    {
        private readonly object _listOperationLock = new object();

        public List<LicenseUser> GetUsersAndGroups(ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("collecting user information from active directory (this process can take some time).");
            return AllUsers(childProgressBar);
        }

        /// <summary>
        ///     Returns a list of all the users from Active Directory.
        /// </summary>
        /// <param name="childProgressBar"></param>
        /// <returns></returns>
        private List<LicenseUser> AllUsers(ChildProgressBar childProgressBar)
        {            
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                using (var search = new PrincipalSearcher(new UserPrincipal(context)))
                {                   
                    var allUsers = search.FindAll().Cast<UserPrincipal>().ToList();
                    List<LicenseUser> users = new List<LicenseUser>(allUsers.Count);

                    using (var pbar = Environment.UserInteractive ? childProgressBar.Spawn(allUsers.Count, "getting users", new ProgressBarOptions
                    {
                        ForeGroundColor = ConsoleColor.Yellow,
                        ProgressCharacter = '─',
                        BackgroundColor = ConsoleColor.DarkGray,
                    }) : null)
                    {
                        Parallel.ForEach(allUsers, user =>
                        {
                            lock (_listOperationLock)
                            {
                                users.Add(new LicenseUser
                                {
                                    DisplayName = user.DisplayName,
                                    Email = user.EmailAddress,
                                    Enabled = user.Enabled ?? false,
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
                                });

                                pbar?.Tick($"found: {user.DisplayName}");
                            }
                        });
                    }
                    childProgressBar?.Tick();
                    return users;
                }
            }                        
        }
    }
}