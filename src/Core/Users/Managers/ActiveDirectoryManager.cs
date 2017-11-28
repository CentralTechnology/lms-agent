namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using Abp.Domain.Services;
    using Abp.Extensions;
    using Common.Extensions;
    using Dto;
    using Extensions;
    using Microsoft.OData.Client;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using ServiceStack.Text;

    public class ActiveDirectoryManager : DomainService, IActiveDirectoryManager
    {

        /// <summary>
        ///     Returns a list of all the users from Active Directory.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public IEnumerable<LicenseUser> AllUsers()
        {
            var context = new PrincipalContext(ContextType.Domain);
            Logger.Debug("Context created.");

            // get the groups first

            using (var groupSearch = new PrincipalSearcher(new GroupPrincipal(context)))
            {
                Logger.Debug("Searching groups.");

                var groups = new List<GroupPrincipalOutput>();
                foreach (Principal found in groupSearch.FindAll())
                {
                    try
                    {
                        var adGroup = found as GroupPrincipal;

                        if (adGroup?.Guid == null)
                        {
                            continue;
                        }
                        if (adGroup.IsSecurityGroup == null || (bool)!adGroup.IsSecurityGroup)
                        {
                            continue;
                        }

                        var members = new List<Guid>();
                        try
                        {
                            members = adGroup.Members.Where(m => m.Guid != null).Select(m => (Guid)m.Guid).ToList();
                        }
                        catch (PrincipalOperationException ex)
                        {
                            Logger.Error($"Failed to get members from group: {(adGroup.DisplayName.IsNullOrEmpty() ? adGroup.SamAccountName : adGroup.DisplayName)}. " +
                                "Most likely cause is that the group contains members which are no longer part of the domain. " +
                                "Please investigate the group members in Active Directory.");
                            Logger.Debug("Exception", ex);
                        }

                        if (adGroup.Guid == null)
                        {
                            Logger.Debug($"Group: {adGroup.Name} does not have a valid GUID - skipping");
                            continue;
                        }

                        groups.Add(new GroupPrincipalOutput
                        {
                            Id = (Guid)adGroup.Guid,
                            Members = members,
                            Name = adGroup.Name,
                            WhenCreated = DateTime.Parse(adGroup.GetProperty("whenCreated"))
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error processing: {found.Guid.Dump()}. Please use the following powershell command to find the group. Get-ADGroup -Identity {found.Guid}");
                        Logger.Debug("Exception", ex);
                    }
                }

                // then process the users
                using (var userSearch = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    Logger.Debug("Searching users.");

                    List<UserPrincipal> users = userSearch.FindAll()
                        .Cast<UserPrincipal>()
                        .Where(u => u.Guid != null)
                        .ToList();

                    foreach (UserPrincipal user in users)
                    {
                        var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;

                        var localGroups = groups.Where(g => g.Members.Any(m => m == user.Guid))
                            .Select(g => new LicenseGroup
                            {
                                Id = g.Id,
                                Name = g.Name,
                                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                                WhenCreated = DateTimeOffset.Parse(g.WhenCreated.ToString())
                            });

                        yield return new LicenseUser
                        {
                            DisplayName = user.DisplayName,
                            Email = user.EmailAddress,
                            Enabled = !dirEntry.IsAccountDisabled(),
                            FirstName = user.GivenName,
                            Groups = new DataServiceCollection<LicenseGroup>(localGroups, TrackingMode.None),
                            Id = Guid.Parse(user.Guid.ToString()),
                            LastLoginDate = user.LastLogon == null ? (DateTimeOffset?)null : DateTimeOffset.Parse(user.LastLogon.ToString()),
                            SamAccountName = user.SamAccountName,
                            Surname = user.Surname,
                            WhenCreated = DateTimeOffset.Parse(user.GetProperty("whenCreated"))
                        };
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<LicenseUserDto> GetUsers()
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                using (var userPrincipal = new UserPrincipal(principalContext))
                {
                    using (var principalSearcher = new PrincipalSearcher(userPrincipal))
                    {
                        using (var results = principalSearcher.FindAll())
                        {
                            foreach (var principal in results)
                            {
                                if (principal.Guid == null)
                                {
                                    Logger.Debug($"Cannot process {principal.Name} because the Id is null. Please check this manually in Active Directory.");
                                    continue;
                                }

                                bool validId = Guid.TryParse(principal.Guid.ToString(), out Guid principalId);
                                if (!validId)
                                {
                                    Logger.Debug($"Cannot process {principal.Name} because the Id is not valid. Please check this manually in Active Directory.");
                                    continue;
                                }

                                UserPrincipal user;

                                try
                                {
                                    user = (UserPrincipal)principal;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Failed to cast Principle to UserPrincipal for object {principal.Dump()}");
                                    Logger.Error("Exception while casting to UserPrincipal", ex);
                                    throw;
                                }

                                Logger.Debug($"Retrieving {user.GetDisplayText()} from Active Directory.");

                                bool isAccountDisabled;
                                try
                                {
                                    var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;
                                    isAccountDisabled = !dirEntry.IsAccountDisabled();
                                }
                                catch (Exception ex)
                                {
                                    isAccountDisabled = true;
                                    Logger.Error($"Failed to determine whether {user.GetDisplayText()} is enabled or not. Therefore we have to assumed they are enabled.");
                                    Logger.Debug("Exception getting DirectoryEntry status", ex);
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
                                        Logger.Debug($"Failed to determine the last logon date for {user.GetDisplayText()}. Therefore we have to assume they have never logged on.");
                                    }
                                }

                                DateTimeOffset whenCreated;
                                try
                                {
                                    var getWhenCreated = user.GetProperty("whenCreated");
                                    if (getWhenCreated.IsNullOrEmpty())
                                    {
                                        throw new NullReferenceException($"WhenCreated property for {user.GetDisplayText()} is null or empty. Please make sure the service is running with correct permissions to access Active Directory.");
                                    }

                                    whenCreated = DateTimeOffset.Parse(getWhenCreated);
                                }
                                catch (NullReferenceException nullRef)
                                {
                                    Logger.Error(nullRef.Message);
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Failed to determine the when created date for {user.GetDisplayText()}. Task cannot continue.");
                                    Logger.Debug("Exception getting WhenCreated UserPrincipal property.", ex);
                                    throw;
                                }

                                yield return new LicenseUserDto
                                {
                                    DisplayName = user.DisplayName,
                                    Email = user.EmailAddress,
                                    Enabled = isAccountDisabled,
                                    FirstName = user.GivenName,
                                    Id = principalId,
                                    LastLogon = lastLogon,
                                    SamAccountName = user.SamAccountName,
                                    Surname = user.Surname,
                                    WhenCreated = whenCreated
                                };
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<LicenseGroupDto> GetGroups()
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                using (var groupPrincipal = new GroupPrincipal(principalContext))
                {
                    using (var principalSearcher = new PrincipalSearcher(groupPrincipal))
                    {
                        using (var results = principalSearcher.FindAll())
                        {
                            foreach (var principal in results)
                            {
                                if (principal.Guid == null)
                                {
                                    Logger.Debug($"Cannot process {principal.Name} because the Id is null. Please check this manually in Active Directory.");
                                    continue;
                                }

                                bool validId = Guid.TryParse(principal.Guid.ToString(), out Guid principalId);
                                if (!validId)
                                {
                                    Logger.Debug($"Cannot process {principal.Name} because the Id is not valid. Please check this manually in Active Directory.");
                                    continue;
                                }

                                GroupPrincipal group;

                                try
                                {
                                    group = (GroupPrincipal) principal;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Failed to cast Principal to GroupPrincipal for object {principal.Dump()}");
                                    Logger.Error("Exception while casting to GroupPrincipal", ex);
                                    throw;
                                }

                                Logger.Debug($"Retrieving {group.GetDisplayText()} from Active Directory.");

                                if (group.IsSecurityGroup == null)
                                {
                                    Logger.Warn($"Cannot tell if {group.GetDisplayText()} is a security group or not.");
                                    continue;
                                }


                                bool isValidSecurityGroup = bool.TryParse(group.IsSecurityGroup.ToString(), out bool isSecurityGroup);
                                if (!isValidSecurityGroup)
                                {
                                    Logger.Warn($"Cannot process {group.GetDisplayText()} because the IsSecurityGroup value is not valid");
                                    continue;
                                }


                                DateTimeOffset whenCreated;
                                try
                                {
                                    var getWhenCreated = group.GetProperty("whenCreated");
                                    if (getWhenCreated.IsNullOrEmpty())
                                    {
                                        throw new NullReferenceException($"WhenCreated property for {group.GetDisplayText()} is null or empty. Please make sure the service is running with correct permissions to access Active Directory.");
                                    }

                                    whenCreated = DateTimeOffset.Parse(getWhenCreated);
                                }
                                catch (NullReferenceException nullRef)
                                {
                                    Logger.Error(nullRef.Message);
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Failed to determine the when created date for {group.GetDisplayText()}. Task cannot continue.");
                                    Logger.Debug("Exception getting WhenCreated GroupPrincipal property.", ex);
                                    throw;
                                }

                                yield return new LicenseGroupDto
                                {
                                    Id = principalId,
                                    Name = group.Name,
                                    WhenCreated = whenCreated
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}