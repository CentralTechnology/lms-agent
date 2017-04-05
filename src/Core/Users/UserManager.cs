namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using Abp.Domain.Services;
    using Common;
    using Models;
    using Newtonsoft.Json;
    using ShellProgressBar;

    public class UserManager : DomainService, IUserManager
    {
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
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    using (PrincipalSearcher search = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        List<UserPrincipal> allUsers = search.FindAll().Cast<UserPrincipal>().ToList();
                        List<LicenseUser> users = new List<LicenseUser>(allUsers.Count);

                        using (ChildProgressBar pbar = Environment.UserInteractive
                            ? childProgressBar.Spawn(allUsers.Count, "getting users", new ProgressBarOptions
                            {
                                ForeGroundColor = ConsoleColor.Yellow,
                                ProgressCharacter = '─',
                                BackgroundColor = ConsoleColor.DarkGray
                            })
                            : null)
                        {
                            foreach (UserPrincipal user in allUsers)
                            {
                                try
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
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"There was a problem processing {user.Name}. Skipping");
                                    Logger.Debug($"Could not convert the following UserPrinciple into a User object {JsonConvert.SerializeObject(user, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore})}");
                                    Logger.Debug(ex.ToString());
                                }
                                finally
                                {
                                    pbar?.Tick($"found: {user.DisplayName}");
                                }                              
                            }
                        }
                        childProgressBar?.Tick();
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