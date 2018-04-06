namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.ActiveDirectory;
    using System.Linq;
    using System.Net.NetworkInformation;
    using Abp;
    using Abp.Domain.Services;
    using Abp.Extensions;
    using Common.Extensions;
    using Core.Common.Extensions;
    using Dto;
    using Extensions;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class ActiveDirectoryManager : DomainService, IActiveDirectoryManager
    {
        public LicenseUser GetUserByPrincipalName(PerformContext performContext, string principalName)
        {
            return GetUser(performContext, IdentityType.UserPrincipalName, principalName);
        }

        public LicenseUser GetUser(PerformContext performContext, IdentityType type, string key)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(principalContext, type, key);
                if (user == null)
                {
                    throw new AbpException($"Cannot find User Principal with {type} {key}");
                }

                user.Validate(performContext);

                Logger.Debug(performContext, $"Retrieving {user.GetDisplayText()} from Active Directory.");

                bool enabled = GetUserStatus(user);
                DateTimeOffset? lastLogon = GetLastLogonDate(user);
                DateTimeOffset whenCreated = GetWhenCreated(user);

                if (user.Guid != null)
                {
                    return new LicenseUser
                    {
                        DisplayName = user.DisplayName,
                        Email = user.EmailAddress,
                        Enabled = enabled,
                        FirstName = user.GivenName,
                        Id = user.Guid.Value,
                        LastLoginDate = lastLogon,
                        SamAccountName = user.SamAccountName,
                        Surname = user.Surname,
                        WhenCreated = whenCreated
                    };
                }

                return null;
            }
        }

        public IEnumerable<LicenseUser> GetAllUsers(PerformContext performContext)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                using (var userPrincipal = new UserPrincipal(principalContext))
                {
                    using (var principalSearcher = new PrincipalSearcher(userPrincipal))
                    {
                        using (PrincipalSearchResult<Principal> results = principalSearcher.FindAll())
                        {
                            foreach (Principal principal in results)
                            {
                                LicenseUser localUser = GetUserById(performContext, principal.Guid);
                                if (localUser == null)
                                {
                                    continue;
                                }

                                yield return localUser;
                            }
                        }
                    }
                }
            }
        }

        public List<LicenseUser> GetAllUsersList(PerformContext performContext)
        {
            return GetAllUsers(performContext).ToList();
        }

        public List<LicenseGroup> GetAllGroupsList(PerformContext performContext)
        {
            return GetAllGroups(performContext).ToList();
        }

        public LicenseUser GetUserById(PerformContext performContext, Guid? userId)
        {
            return GetUser(performContext, IdentityType.Guid, userId.ToString());
        }

        public IEnumerable<LicenseGroup> GetAllGroups(PerformContext performContext)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                using (var groupPrincipal = new GroupPrincipal(principalContext))
                {
                    using (var principalSearcher = new PrincipalSearcher(groupPrincipal))
                    {
                        using (PrincipalSearchResult<Principal> results = principalSearcher.FindAll())
                        {
                            foreach (Principal principal in results)
                            {
                                if (principal.Guid == null)
                                {
                                    Logger.Debug(performContext, $"Cannot process {principal.Name} because the Id is null. Please check this manually in Active Directory.");
                                    continue;
                                }

                                bool validId = Guid.TryParse(principal.Guid.ToString(), out Guid principalId);
                                if (!validId)
                                {
                                    Logger.Debug(performContext, $"Cannot process {principal.Name} because the Id is not valid. Please check this manually in Active Directory.");
                                    continue;
                                }

                                if (!(principal is GroupPrincipal group))
                                {
                                    continue;
                                }

                                Logger.Debug(performContext, $"Retrieving {group.GetDisplayText()} from Active Directory.");

                                LicenseGroup localGroup = GetGroup(performContext, principalId);
                                if (localGroup == null)
                                {
                                    continue;
                                }

                                yield return localGroup;
                            }
                        }
                    }
                }
            }
        }

        public LicenseGroup GetGroup(PerformContext performContext, Guid groupId)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                GroupPrincipal group = GetGroupPrincipal(principalContext, groupId);

                if (group.IsSecurityGroup == null)
                {
                    Logger.Warn($"Cannot tell if {group.GetDisplayText()} is a security group or not.");
                    return null;
                }

                if (!bool.TryParse(group.IsSecurityGroup.ToString(), out bool _))
                {
                    Logger.Warn($"Cannot process {group.GetDisplayText()} because the IsSecurityGroup value is not valid");
                    return null;
                }

                DateTimeOffset whenCreated;
                try
                {
                    string getWhenCreated = group.GetProperty("whenCreated");
                    if (getWhenCreated.IsNullOrEmpty())
                    {
                        throw new NullReferenceException($"WhenCreated property for {group.GetDisplayText()} is null or empty. Please make sure the service is running with correct permissions to access Active Directory.");
                    }

                    whenCreated = DateTimeOffset.Parse(getWhenCreated);
                }
                catch (NullReferenceException nullRef)
                {
                    Logger.Error(performContext, nullRef.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.Error(performContext, $"Failed to determine the when created date for {group.GetDisplayText()}. Task cannot continue.");
                    Logger.Debug(performContext, "Exception getting WhenCreated GroupPrincipal property.", ex);
                    throw;
                }

                return new LicenseGroup
                {
                    Id = groupId,
                    Name = group.Name,
                    WhenCreated = whenCreated
                };
            }
        }

        public List<LicenseUserGroup> GetGroupMembers(PerformContext performContext, Guid groupId)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                GroupPrincipal group = GetGroupPrincipal(principalContext, groupId);

                var licenseGroupUsers = new List<LicenseUserGroup>();

                try
                {
                    using (PrincipalSearchResult<Principal> members = group.GetMembers())
                    {
                        if (!members.Any())
                        {
                            return licenseGroupUsers;
                        }

                        foreach (Principal principal in members)
                        {
                            var user = principal.Validate(performContext);
                            if (user?.Guid == null)
                            {
                                continue;
                            }

                            LicenseUser localUser = GetUserById(performContext, user.Guid.Value);
                            if (localUser == null)
                            {
                                continue;
                            }

                            licenseGroupUsers.Add(new LicenseUserGroup
                            {
                                GroupId = groupId,
                                UserId = localUser.Id
                            });
                        }

                        return licenseGroupUsers;
                    }
                }
                catch (PrincipalOperationException ex)
                {
                    Logger.Error($"Group: {group.Name} has some invalid members. This will need to be manually corrected in Active Directory.");
                    Logger.Debug(ex.Message, ex);
                    return licenseGroupUsers;
                }
            }
        }

        public bool IsOnDomain(PerformContext performContext)
        {
            try
            {
                using (new PrincipalContext(ContextType.Domain))
                {
                    return true;
                }
            }
            catch (ActiveDirectoryOperationException ex)
            {
                Logger.Debug(performContext, ex.Message, ex);
                return false;
            }
        }

        public bool IsPrimaryDomainController(PerformContext performContext)
        {
            try
            {
                Domain domain = Domain.GetCurrentDomain();
                DomainController primaryDomainController = domain.PdcRoleOwner;

                string currentMachine = $"{Environment.MachineName}.{IPGlobalProperties.GetIPGlobalProperties().DomainName}";

                return primaryDomainController.Name.Equals(currentMachine, StringComparison.OrdinalIgnoreCase);
            }
            catch (ActiveDirectoryOperationException ex)
            {
                Logger.Debug(performContext, ex.Message, ex);
                return false;
            }
        }

        private long ConvertActiveDirectoryLargeIntegerToLong(object adsLargeInteger)
        {
            try
            {
                var highPart = (int) adsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
                var lowPart = (int) adsLargeInteger.GetType().InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
                return highPart * ((long) uint.MaxValue + 1) + lowPart;
            }
            catch (Exception ex)
            {
                Logger.Debug("Error converting active directory DateTime to Epoch Time.", ex);
                return default(long);
            }
        }

        private GroupPrincipal GetGroupPrincipal(PrincipalContext principalContext, Guid id)
        {
            GroupPrincipal group = GroupPrincipal.FindByIdentity(principalContext, IdentityType.Guid, id.ToString());
            if (group == null)
            {
                throw new AbpException($"Cannot find Group Principal with Guid {id}");
            }

            return group;
        }

        private DateTimeOffset? GetLastLogonDate(UserPrincipal user)
        {
            try
            {
                if (!(user.GetUnderlyingObject() is DirectoryEntry dirEntry))
                {
                    return null;
                }

                if (dirEntry.Properties["lastLogon"].Value != null)
                {
                    var adDateTime = ConvertActiveDirectoryLargeIntegerToLong(dirEntry.Properties["lastLogon"].Value);
                    if (adDateTime != default(long))
                    {
                        return DateTimeOffset.FromFileTime(adDateTime);
                    }
                }

                if (DateTimeOffset.TryParse(user.LastLogon.ToString(), out DateTimeOffset lastLogonValue))
                {
                    return lastLogonValue;
                }

                Logger.Debug($"Failed to determine the last logon date for {user.GetDisplayText()}. Therefore we have to assume they have never logged on.");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return null;
            }
        }

        private bool GetUserStatus(UserPrincipal user)
        {
            try
            {
                if (!(user.GetUnderlyingObject() is DirectoryEntry dirEntry))
                {
                    return true;
                }

                return !dirEntry.IsAccountDisabled();
            }
            catch (Exception ex)
            {
                Logger.Debug($"Failed to determine the account status for {user.GetDisplayText()}. Therefore we have to assume they are Enabled.", ex);

                // always assume they are enabled
                return true;
            }
        }

        private DateTimeOffset GetWhenCreated(UserPrincipal user)
        {
            try
            {
                if (!(user.GetUnderlyingObject() is DirectoryEntry dirEntry))
                {
                    throw new AbpException($"Failed to determine the when created date for {user.GetDisplayText()}.");
                }

                if (dirEntry.Properties["whenCreated"].Value != null)
                {
                    if (DateTimeOffset.TryParse(dirEntry.Properties["whenCreated"].Value.ToString(), out DateTimeOffset whenCreated))
                    {
                        return whenCreated;
                    }
                }

                throw new AbpException($"Failed to determine the when created date for {user.GetDisplayText()}.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
    }
}