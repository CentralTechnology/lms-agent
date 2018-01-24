namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.ActiveDirectory;
    using System.Linq;
    using System.Net.NetworkInformation;
    using Abp;
    using Abp.Domain.Services;
    using Abp.Extensions;
    using Common.Extensions;
    using Dto;
    using Extensions;
    using global::Hangfire.Server;

    public class ActiveDirectoryManager : DomainService, IActiveDirectoryManager
    {
        public LicenseUserDto GetUserByPrincipalName(PerformContext performContext, string principalName)
        {
            return GetUser(performContext, IdentityType.UserPrincipalName, principalName);
        }

        public LicenseUserDto GetUser(PerformContext performContext, IdentityType type, string key)
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

                bool isAccountDisabled;
                try
                {
                    var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;
                    isAccountDisabled = !dirEntry.IsAccountDisabled();
                }
                catch (Exception ex)
                {
                    isAccountDisabled = true;
                    Logger.Error(performContext, $"Failed to determine whether {user.GetDisplayText()} is enabled or not. Therefore we have to assumed they are enabled.");
                    Logger.Debug(performContext, "Exception getting DirectoryEntry status", ex);
                }

                DateTimeOffset? lastLogon = null;
                if (user.LastLogon != null)
                {
                    bool validLastLogon = DateTimeOffset.TryParse(user.LastLogon.ToString(), out DateTimeOffset lastLogonValue);
                    if (validLastLogon)
                    {
                        lastLogon = lastLogonValue;
                    }
                    else
                    {
                        Logger.Debug(performContext, $"Failed to determine the last logon date for {user.GetDisplayText()}. Therefore we have to assume they have never logged on.");
                    }
                }

                DateTimeOffset whenCreated;
                try
                {
                    string getWhenCreated = user.GetProperty("whenCreated");
                    if (getWhenCreated.IsNullOrEmpty())
                    {
                        throw new NullReferenceException($"WhenCreated property for {user.GetDisplayText()} is null or empty. Please make sure the service is running with correct permissions to access Active Directory.");
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
                    Logger.Error(performContext, $"Failed to determine the when created date for {user.GetDisplayText()}. Task cannot continue.");
                    Logger.Debug(performContext, "Exception getting WhenCreated UserPrincipal property.", ex);
                    throw;
                }

                return new LicenseUserDto
                {
                    DisplayName = user.DisplayName,
                    Email = user.EmailAddress,
                    Enabled = isAccountDisabled,
                    FirstName = user.GivenName,
                    // ReSharper disable once PossibleInvalidOperationException
                    Id = user.Guid.Value,
                    LastLoginDate = lastLogon,
                    SamAccountName = user.SamAccountName,
                    Surname = user.Surname,
                    WhenCreated = whenCreated
                };
            }
        }

        /// <param name="performContext"></param>
        /// <inheritdoc />
        public IEnumerable<LicenseUserDto> GetUsers(PerformContext performContext)
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
                                LicenseUserDto localUser = GetUserById(performContext, principal.Guid);
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

        public LicenseUserDto GetUserById(PerformContext performContext, Guid? userId)
        {
            return GetUser(performContext, IdentityType.Guid, userId.ToString());
        }

        /// <param name="performContext"></param>
        /// <inheritdoc />
        public IEnumerable<LicenseGroupDto> GetGroups(PerformContext performContext)
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

                                LicenseGroupDto localGroup = GetGroup(performContext, principalId);
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

        private GroupPrincipal GetGroupPrincipal(PrincipalContext principalContext, Guid id)
        {
            GroupPrincipal group = GroupPrincipal.FindByIdentity(principalContext, IdentityType.Guid, id.ToString());
            if (group == null)
            {
                throw new AbpException($"Cannot find Group Principal with Guid {id}");
            }

            return group;
        }

        public LicenseGroupDto GetGroup(PerformContext performContext, Guid groupId)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                GroupPrincipal group = GetGroupPrincipal(principalContext, groupId);

                if (group.IsSecurityGroup == null)
                {
                    Logger.Warn($"Cannot tell if {group.GetDisplayText()} is a security group or not.");
                    return null;
                }

                bool isValidSecurityGroup = bool.TryParse(group.IsSecurityGroup.ToString(), out bool isSecurityGroup);
                if (!isValidSecurityGroup)
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

                return new LicenseGroupDto
                {
                    Id = groupId,
                    Name = group.Name,
                    WhenCreated = whenCreated
                };
            }
        }

        /// <inheritdoc />
        public LicenseGroupUsersDto GetGroupMembers(PerformContext performContext, Guid groupId)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                GroupPrincipal group = GetGroupPrincipal(principalContext, groupId);

                var licenseGroupUsers = new LicenseGroupUsersDto(groupId, group.Name);

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

                            LicenseUserDto localUser = GetUserById(performContext, user.Guid.Value);
                            if (localUser == null)
                            {
                                continue;
                            }

                            licenseGroupUsers.Users.Add(localUser);
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
                using (var principalContext = new PrincipalContext(ContextType.Domain))
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
    }
}