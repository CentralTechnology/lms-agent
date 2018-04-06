namespace LMS.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.UI;
    using Core.Extensions;
    using Core.Extensions.Helpers;
    using Core.Extensions.Managers;
    using Core.Services;
    using Core.Services.Authentication;
    using Core.Users.Compare;
    using global::Hangfire.Server;
    using Hangfire;
    using Managers;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserWorkerManager : WorkerManagerBase, IUserWorkerManager
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly LicenseGroupEqualityComparer _licenseGroupEqualityComparer = new LicenseGroupEqualityComparer();
        private readonly LicenseUserEqualityComparer _licenseUserEqualityComparer = new LicenseUserEqualityComparer();
        private readonly LicenseUserGroupEqualityComparer _licenseUserGroupEqualityComparer = new LicenseUserGroupEqualityComparer();

        public UserWorkerManager(
            IPortalService portalService,
            IPortalAuthenticationService authService,
            IActiveDirectoryManager activeDirectoryManager
        ) : base(portalService, authService)
        {
            _activeDirectoryManager = activeDirectoryManager;
        }

        public async Task ComputeUsers(PerformContext performContext, int managedSupportId)
        {
            Logger.Info(performContext, "Getting the users from Active Directory...");

            var adUsers = _activeDirectoryManager.GetAllUsersList(performContext);

            List<LicenseUser> remoteUsers = (await PortalService.GetAllUsersAsync()).ToList();

            foreach (var adUser in adUsers)
            {
                performContext?.Cancel();

                var remoteUser = remoteUsers.FirstOrDefault(ru => ru.Id == adUser.Id);
                if (remoteUser == null)
                {
                    var newUser = LicenseUser.Create(
                        adUser,
                        managedSupportId,
                        AuthService.GetAccount());

                    await PortalService.AddUserAsync(newUser);

                    performContext?.WriteSuccessLine($"+ {newUser}");
                    Logger.Info($"Created: {newUser}");
                    Logger.Debug($"{JsonConvert.SerializeObject(newUser, Formatting.Indented)}");

                    continue;
                }

                remoteUser.UpdateValues(adUser);
                await PortalService.UpdateUserAsync(remoteUser);

                performContext?.WriteSuccessLine($"+ {remoteUser}");
                Logger.Info($"Updated: {remoteUser}");
                Logger.Debug($"{JsonConvert.SerializeObject(remoteUser, Formatting.Indented)}");
            }

            var staleUsers = remoteUsers.Except(adUsers, _licenseUserEqualityComparer).ToList();
            foreach (var staleUser in staleUsers)
            {
                performContext?.Cancel();

                if (staleUser.IsDeleted)
                {
                    continue;
                }

                await PortalService.DeleteUserAsync(staleUser);

                performContext?.WriteWarnLine($"- {staleUser}");
                Logger.Info($"Delete: {staleUser}");
                Logger.Debug($"{JsonConvert.SerializeObject(staleUser, Formatting.Indented)}");
            }
        }

        public async Task ComputeGroupMembershipAsync(PerformContext performContext)
        {
            Logger.Info(performContext, "Getting the group from Active Directory...");

            var groups = _activeDirectoryManager.GetAllGroupsList(performContext);
            foreach (var group in groups)
            {
                performContext?.Cancel();

                var groupMembers = _activeDirectoryManager.GetGroupMembers(performContext, group.Id);

                var userGroups = PortalService.GetAllGroupUsers(group.Id);

                var newMembers = groupMembers.Except(userGroups, _licenseUserGroupEqualityComparer);
                foreach (var newMember in newMembers)
                {
                    performContext?.Cancel();

                    await PortalService.AddUserGroupAsync(
                        LicenseUserGroup.Create(group.Id, newMember.UserId));

                    performContext?.WriteSuccessLine($"+ {group.Name}  {newMember.UserId}");
                    Logger.Info($"User: {newMember.UserId} was added to Group: {group.Id} {group.Name}");
                }

                var staleMembers = userGroups.Except(groupMembers, _licenseUserGroupEqualityComparer).ToList();
                foreach (var staleMember in staleMembers)
                {
                    performContext?.Cancel();

                    await PortalService.DeleteUserGroupAsync(
                        staleMember);

                    performContext?.WriteWarnLine($"+ {group.Name}  {staleMember.UserId}");
                    Logger.Info($"User: {staleMember.UserId} was removed from Group: {group.Id} {group.Name}");
                }
            }
        }

        public async Task ComputeGroups(PerformContext performContext)
        {
            List<LicenseGroup> remoteGroups = (await PortalService.GetAllGroupsAsync()).ToList();

            Logger.Info(performContext, "Getting the groups from Active Directory...");

            var adGroups = _activeDirectoryManager.GetAllGroupsList(performContext);

            foreach (var adGroup in adGroups)
            {
                performContext?.Cancel();

                var remoteGroup = remoteGroups.FirstOrDefault(rg => rg.Id == adGroup.Id);
                if (remoteGroup == null)
                {
                    var newGroup = LicenseGroup.Create(
                        adGroup,
                        AuthService.GetAccount());

                    await PortalService.AddGroupAsync(newGroup);

                    performContext?.WriteSuccessLine($"+ {newGroup}");
                    Logger.Info($"Created: {newGroup}");
                    Logger.Debug($"{JsonConvert.SerializeObject(newGroup, Formatting.Indented)}");

                    continue;
                }

                remoteGroup.UpdateValues(adGroup);
                await PortalService.UpdateGroupAsync(remoteGroup);

                performContext?.WriteSuccessLine($"+ {remoteGroup}");
                Logger.Info($"Updated:  {remoteGroup}");
                Logger.Debug($"{JsonConvert.SerializeObject(remoteGroup, Formatting.Indented)}");
            }

            var staleGroups = remoteGroups.Except(adGroups, _licenseGroupEqualityComparer).ToList();
            foreach (var staleGroup in staleGroups)
            {
                performContext?.Cancel();

                if (staleGroup.IsDeleted)
                {
                    continue;
                }

                await PortalService.DeleteGroupAsync(staleGroup);

                performContext?.WriteWarnLine($"- {staleGroup}");
                Logger.Info($"Delete: {staleGroup}");
                Logger.Debug($"{JsonConvert.SerializeObject(staleGroup, Formatting.Indented)}");
            }
        }

        [Mutex("UserWorkerManager")]
        public override async Task StartAsync(PerformContext performContext)
        {
            await ExecuteAsync(performContext, async () =>
            {
                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "Acquiring the upload details from the api.");

                var managedServer = PortalService.GetManagedServer();
                if (managedServer == null)
                {
                    Logger.Info(performContext, "It appears this device hasn't logged in before. Generating the upload details.");

                    try
                    {
                        var upload = ManagedSupport.Create(
                            SettingManagerHelper.ClientVersion,
                            AuthService.GetDevice(),
                            AuthService.GetAccount());

                        await PortalService.AddManagedServerAsync(upload);

                        managedServer = PortalService.GetManagedServer();
                        if (managedServer == null)
                        {
                            throw new UserFriendlyException("There was a problem creating the upload, please try again. If the problem persists, please raise a bug report.");
                        }
                    }
                    catch (Exception ex) when (!(ex is UserFriendlyException))
                    {
                        throw new UserFriendlyException(ex.Message);
                    }
                }

                Logger.Info(performContext, "Upload details acquired!");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "---------- Uploading groups begin ----------");

                await ComputeGroups(performContext);

                Logger.Info(performContext, "---------- Uploading groups end ----------");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "---------- Uploading users begin ----------");

                await ComputeUsers(performContext, managedServer.Id);

                Logger.Info(performContext, "---------- Uploading users end ----------");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "Calculating group memberships. This could take some time...");

                await ComputeGroupMembershipAsync(performContext);

                Logger.Info(performContext, "Group memberships are up to date.");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "Completing the upload.");

                await PortalService.UpdateManagedServerAsync(managedServer);
            });
        }
    }
}