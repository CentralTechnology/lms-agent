namespace LMS.Core.Users
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.UI;
    using Compare;
    using Core.Extensions;
    using Core.Managers;
    using global::Hangfire.Console;
    using global::Hangfire.Server;
    using Hangfire;
    using Helpers;
    using Managers;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Serilog;
    using Services;
    using Services.Authentication;

    public class UserWorkerManager : WorkerManagerBase, IUserWorkerManager
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly LicenseGroupEqualityComparer _licenseGroupEqualityComparer = new LicenseGroupEqualityComparer();
        private readonly LicenseUserEqualityComparer _licenseUserEqualityComparer = new LicenseUserEqualityComparer();
        private readonly LicenseUserGroupEqualityComparer _licenseUserGroupEqualityComparer = new LicenseUserGroupEqualityComparer();

        private readonly ILogger _logger = Log.ForContext<UserWorkerManager>();

        public UserWorkerManager(
            IPortalService portalService,
            IActiveDirectoryManager activeDirectoryManager
        ) : base(portalService)
        {
            _activeDirectoryManager = activeDirectoryManager;
        }

        public async Task ComputeUsers(PerformContext performContext, int managedSupportId)
        {
            performContext?.WriteLine("Getting the users from the Portal...");
            var remoteUsers = PortalService.GetAllUsers().ToArray();

            performContext?.WriteLine("Getting the users from Active Directory...");
            var adUsers = _activeDirectoryManager.GetAllUsers().ToList();

            foreach (var adUser in adUsers)
            {
                performContext?.Cancel();

                var remoteUser = remoteUsers.FirstOrDefault(ru => ru.Id == adUser.Id);
                if (remoteUser == null)
                {
                    var newUser = LicenseUser.Create(
                        adUser,
                        managedSupportId,
                        PortalAuthenticationService.Instance.GetAccount());

                    await PortalService.AddUserAsync(newUser);

                    performContext?.WriteSuccessLine($"+ {newUser}");
                    _logger.Information("Created: {User}", newUser.ToString());
                    _logger.Debug("Created: {@User}", newUser);

                    continue;
                }

                remoteUser.UpdateValues(adUser);
                await PortalService.UpdateUserAsync(remoteUser);

                performContext?.WriteSuccessLine($"^ {remoteUser}");
                _logger.Information("Updated: {User}", remoteUser.ToString());
                _logger.Debug("Updated: {@User}", remoteUser);
            }

            var staleUsers = remoteUsers.Except(adUsers, _licenseUserEqualityComparer).ToArray();
            foreach (var staleUser in staleUsers)
            {
                performContext?.Cancel();

                if (staleUser.IsDeleted)
                {
                    continue;
                }

                await PortalService.DeleteUserAsync(staleUser);

                performContext?.WriteWarnLine($"- {staleUser}");
                _logger.Information("Delete: {staleUser}", staleUser.ToString());
                _logger.Debug("Delete: {@staleUser}", staleUser);
            }
        }

        public async Task ComputeGroupMembershipAsync(PerformContext performContext)
        {
            performContext?.WriteLine("Getting the groups from Active Directory...");

            var groups = _activeDirectoryManager.GetAllGroups().ToArray();
            foreach (var group in groups)
            {
                performContext?.Cancel();

                var groupMembers = _activeDirectoryManager.GetGroupMembers(group.Id);

                var userGroups = PortalService.GetAllGroupUsers(group.Id).ToArray();

                var newMembers = groupMembers.Except(userGroups, _licenseUserGroupEqualityComparer).ToArray();
                foreach (var newMember in newMembers)
                {
                    performContext?.Cancel();

                    await PortalService.AddUserGroupAsync(
                        LicenseUserGroup.Create(group.Id, newMember.UserId));

                    performContext?.WriteSuccessLine($"+ {group.Name}  {newMember.UserId}");
                    _logger.Information("User: {UserId} was added to Group: {GroupId} {GroupName}", newMember.UserId, group.Id, group.Name);
                }

                var staleMembers = userGroups.Except(groupMembers, _licenseUserGroupEqualityComparer).ToArray();
                foreach (var staleMember in staleMembers)
                {
                    performContext?.Cancel();

                    await PortalService.DeleteUserGroupAsync(
                        staleMember);

                    performContext?.WriteWarnLine($"- {group.Name}  {staleMember.UserId}");
                    _logger.Information("User: {UserId} was removed from Group: {GroupId} {GroupName}", staleMember.UserId, group.Id, group.Name);
                }

                if (!newMembers.Any() && !staleMembers.Any())
                {
                    _logger.Information("Group: {GroupId}  No changes have been made.", group.Id);
                }
                else
                {
                    _logger.Information("Group: {GroupId}  Added: {NewMembersCount}  Removed: {StaleMembersCount}", group.Id, newMembers.Length, staleMembers.Length);
                }
            }
        }

        public async Task ComputeGroups(PerformContext performContext)
        {
            performContext?.WriteLine("Getting the groups from the Portal...");
            var remoteGroups = PortalService.GetAllGroups().ToArray();

            performContext?.WriteLine("Getting the groups from Active Directory...");
            var adGroups = _activeDirectoryManager.GetAllGroups().ToArray();

            foreach (var adGroup in adGroups)
            {
                performContext?.Cancel();

                var remoteGroup = remoteGroups.FirstOrDefault(rg => rg.Id == adGroup.Id);
                if (remoteGroup == null)
                {
                    var newGroup = LicenseGroup.Create(
                        adGroup,
                        PortalAuthenticationService.Instance.GetAccount());

                    await PortalService.AddGroupAsync(newGroup);

                    performContext?.WriteSuccessLine($"+ {newGroup}");
                    _logger.Information("Created: {Group}", newGroup.ToString());
                    _logger.Debug("Created: {@Group}", newGroup);

                    continue;
                }

                remoteGroup.UpdateValues(adGroup);
                await PortalService.UpdateGroupAsync(remoteGroup);

                performContext?.WriteSuccessLine($"^ {remoteGroup}");
                _logger.Information("Updated:  {Group}", remoteGroup.ToString());
                _logger.Debug("Updated:  {@Group}", remoteGroup);
            }

            var staleGroups = remoteGroups.Except(adGroups, _licenseGroupEqualityComparer).ToArray();
            foreach (var staleGroup in staleGroups)
            {
                performContext?.Cancel();

                if (staleGroup.IsDeleted)
                {
                    continue;
                }

                await PortalService.DeleteGroupAsync(staleGroup);

                performContext?.WriteWarnLine($"- {staleGroup}");
                _logger.Information("Delete: {Group}", staleGroup.ToString());
                _logger.Debug("Delete: {@Group}", staleGroup);
            }
        }

        [Mutex("UserWorkerManager")]
        public override async Task StartAsync(PerformContext performContext)
        {
            await ExecuteAsync(performContext, async () =>
            {
                Console.WriteLine(Environment.NewLine);
                performContext?.WriteLine("Acquiring the upload details from the api.");

                var managedServer = PortalService.GetManagedServer();
                if (managedServer == null)
                {
                    performContext?.WriteLine("It appears this device hasn't logged in before. Generating the upload details.");
                    _logger.Information("New LMS Agent detected.");

                    try
                    {
                        var upload = ManagedSupport.Create(
                            SettingManagerHelper.ClientVersion,
                            PortalAuthenticationService.Instance.GetDevice(),
                            PortalAuthenticationService.Instance.GetAccount());

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
                else
                {
                    managedServer.ClientVersion = SettingManagerHelper.ClientVersion;
                }

                performContext.WriteLine("Upload details acquired!");

                Console.WriteLine(Environment.NewLine);
                performContext.WriteLine("---------- Uploading groups begin ----------");

                await ComputeGroups(performContext);

                performContext.WriteLine("---------- Uploading groups end ----------");

                Console.WriteLine(Environment.NewLine);
                performContext.WriteLine("---------- Uploading users begin ----------");

                await ComputeUsers(performContext, managedServer.Id);

                performContext.WriteLine("---------- Uploading users end ----------");

                Console.WriteLine(Environment.NewLine);
                performContext.WriteLine("Calculating group memberships. This could take some time...");

                await ComputeGroupMembershipAsync(performContext);

                performContext.WriteLine("Group memberships are up to date.");

                Console.WriteLine(Environment.NewLine);
                performContext.WriteLine("Completing the upload.");

                await PortalService.UpdateManagedServerAsync(managedServer);
            });
        }
    }
}