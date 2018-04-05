namespace LMS.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.UI;
    using Common.Helpers;
    using Common.Managers;
    using Core.Common.Extensions;
    using Core.Services;
    using Core.Services.Authentication;
    using Dto;
    using global::Hangfire.Server;
    using Hangfire;
    using Managers;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserWorkerManager : WorkerManagerBase, IUserWorkerManager
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;

        public UserWorkerManager(
            IPortalService portalService, 
            IPortalAuthenticationService authService,
            IActiveDirectoryManager activeDirectoryManager
        ) : base(portalService,authService)
        {
            _activeDirectoryManager = activeDirectoryManager;
        }

        //public void ProcessGroups(PerformContext performContext, ManagedSupport managedSupport)
        //{
        //    Console.WriteLine(Environment.NewLine);
        //    Logger.Info(performContext, "--------------- PROCESS GROUPS BEGIN ---------------");

        //    IEnumerable<LicenseGroupDto> groups = _activeDirectoryManager.GetAllGroups(performContext);
        //    List<LicenseGroupSummary> remoteGroups = _portalManager.ListAllGroupIds();
        //    var localGroupIds = new List<Guid>();
        //    foreach (LicenseGroupDto group in groups)
        //    {
        //        performContext?.Cancel();

        //        localGroupIds.Add(group.Id);

        //        bool existingGroup = remoteGroups.Any(ru => ru.Id == group.Id);
        //        if (existingGroup)
        //        {
        //            _groupManager.Update(performContext, group);
        //            continue;
        //        }

        //        _groupManager.Add(performContext, group, managedSupport.TenantId);
        //    }

        //    List<LicenseGroupSummary> activeRemoteGroups = _portalManager.ListAllGroupIds(g => !g.IsDeleted);
        //    IEnumerable<LicenseGroupSummary> groupsToDelete = activeRemoteGroups.Where(ru => localGroupIds.All(u => u != ru.Id));
        //    foreach (LicenseGroupSummary group in groupsToDelete)
        //    {
        //        performContext?.Cancel();

        //        _groupManager.Delete(performContext, group.Id);
        //    }

        //    Logger.Info(performContext, "--------------- PROCESS GROUPS END ---------------");
        //}

        //public void ProcessUserGroups(PerformContext performContext)
        //{
        //    Console.WriteLine(Environment.NewLine);
        //    Logger.Info(performContext, "--------------- PROCESS GROUP MEMBERSHIP BEGIN ---------------");

        //    IEnumerable<LicenseGroupDto> groups = _activeDirectoryManager.GetAllGroups(performContext);
        //    foreach (LicenseGroupDto group in groups)
        //    {
        //        performContext?.Cancel();

        //        LicenseGroupUsersDto localMembers = _activeDirectoryManager.GetGroupMembers(performContext, group.Id);

        //        _userGroupManager.AddUsersToGroup(performContext, localMembers);
        //        _userGroupManager.DeleteUsersFromGroup(performContext, localMembers);

        //        Logger.Info(performContext, $"{group.Name} - Processed");
        //    }

        //    Logger.Info(performContext, "--------------- PROCESS GROUP MEMBERSHIP END ---------------");
        //}

        ///// <summary>
        /////     Decides whether a License User object should be Added, Updated or Deleted from the API.
        ///// </summary>
        ///// <param name="performContext"></param>
        ///// <param name="managedSupport"></param>
        //public void ProcessUsers(PerformContext performContext, ManagedSupport managedSupport)
        //{
        //    Logger.Info(performContext, "--------------- PROCESS USERS BEGIN ---------------");

        //    IEnumerable<LicenseUserDto> users = _activeDirectoryManager.GetAllUsers(performContext);
        //    List<LicenseUserSummary> remoteUsers = _portalManager.ListAllUserIds();
        //    var localUserIds = new List<Guid>();
        //    foreach (LicenseUserDto user in users)
        //    {
        //        performContext?.Cancel();

        //        localUserIds.Add(user.Id);

        //        bool existingUser = remoteUsers.Any(ru => ru.Id == user.Id);
        //        if (existingUser)
        //        {
        //            _userManager.Update(performContext, user);
        //            continue;
        //        }

        //        _userManager.Add(performContext, user, managedSupport.Id, managedSupport.TenantId);
        //    }

        //    List<LicenseUserSummary> activeRemoteUsers = _portalManager.ListAllUserIds(u => !u.IsDeleted);
        //    IEnumerable<LicenseUserSummary> usersToDelete = activeRemoteUsers.Where(ru => localUserIds.All(u => u != ru.Id));
        //    foreach (LicenseUserSummary user in usersToDelete)
        //    {
        //        performContext?.Cancel();

        //        _userManager.Delete(performContext, user.Id);
        //    }

        //    Logger.Info(performContext, " ---------------PROCESS USERS END ---------------");
        //}



        [Mutex("UserWorkerManager")]
        public override void Start(PerformContext performContext)
        {
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();

            //Execute(performContext, () =>
            //{
            //    _startupManager.ValidateCredentials(performContext);

            //    Logger.Info(performContext, "Getting account details from the api.");
            //    ManagedSupport managedSupport = _managedSupportManager.Get() ?? _managedSupportManager.Add(performContext);
            //    _portalManager.Detach(managedSupport);

            //    ProcessUsers(performContext, managedSupport);
            //    ProcessGroups(performContext, managedSupport);
            //    ProcessUserGroups(performContext);

            //    // let the api know we have completed the task
            //    _managedSupportManager.Update(managedSupport);
            //});

            //stopWatch.Stop();
            //var timeTaken = stopWatch.Elapsed;

            //try
            //{
            //    _operationManager.Add(timeTaken.Minutes);
            //    var averageRunTime = _operationManager.Get();
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.UsersAverageRuntime, averageRunTime.ToString());
            //    performContext?.WriteLine($"Average runtime (minutes): {averageRunTime}");
            //}
            //catch (Exception)
            //{
            //    Logger.Error("Failed to update the Average runtime.");
            //    // ignored
            //}
        }

        [Mutex("UserWorkerManager")]
        public override async Task StartAsync(PerformContext performContext)
        {
            await ExecuteAsync(performContext, async () =>
            {
                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "Acquiring the upload details from the api.");

                var managedSupport = PortalService.GetManagedServer();
                if (managedSupport.Count != 1)
                {
                    Logger.Info(performContext, "It appears this device hasn't logged in before. Generating the upload details.");

                    try
                    {
                        var upload = ManagedSupport.Create(SettingManagerHelper.ClientVersion, AuthService.GetDevice());
                        await PortalService.AddManagedServerAsync(upload);
                        managedSupport = PortalService.GetManagedServer();
                        if (managedSupport.Count != 1)
                        {
                            throw new UserFriendlyException("There was a problem creating the upload, please try again. If the problem persists, please raise a bug report.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        throw new UserFriendlyException(ex.Message);
                    }
                }

                Logger.Info(performContext, "Upload details acquired!");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "---------- Uploading users begin ----------");

                await ComputeUsers(performContext, managedSupport[0].Id);

                Logger.Info(performContext, "---------- Uploading users end ----------");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "---------- Uploading groups begin ----------");

                await ComputeGroups(performContext);

                Logger.Info(performContext, "---------- Uploading groups end ----------");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext,"Calculating group memberships. This could take some time...");

                await ComputeGroupMembershipAsync(performContext);

                Logger.Info(performContext, "Group memberships are up to date.");

                Console.WriteLine(Environment.NewLine);
                Logger.Info(performContext, "Completing the upload.");

                await PortalService.UpdateManagedServerAsync(managedSupport);
            });
        }

        public async Task ComputeUsers(PerformContext performContext, int managedSupportId)
        {
            var adUsers = _activeDirectoryManager
                .GetAllUsersList(performContext)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.Surname)
                .ThenBy(u => u.DisplayName)
                .ThenBy(u => u.SamAccountName)
                .ToArray();

            var remoteUsers = await PortalService.GetAllUsersAsync();

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
                Logger.Info($"Updated:  {remoteUser}");
                Logger.Debug($"{JsonConvert.SerializeObject(remoteUser, Formatting.Indented)}");
            }

            var staleUsers = remoteUsers.Where(ru => adUsers.All(au => au.Id != ru.Id)).ToArray();
            foreach (var staleUser in staleUsers)
            {
                performContext?.Cancel();

                await PortalService.DeleteUserAsync(staleUser);

                performContext?.WriteWarnLine($"- {staleUser}");
                Logger.Info($"Delete: {staleUser}");
                Logger.Debug($"{JsonConvert.SerializeObject(staleUser, Formatting.Indented)}");
            }
        }

        public async Task ComputeGroupMembershipAsync(PerformContext performContext)
        {
            var groups = _activeDirectoryManager.GetAllGroupsList(performContext);
            foreach (var group in groups)
            {
                performContext?.Cancel();

                var groupMembers = _activeDirectoryManager.GetGroupMembers(performContext, group.Id);

                var userGroups = PortalService.GetAllGroupUsers(@group.Id);

                var newMembers = new HashSet<LicenseUserDto>(groupMembers.Users.Select(u => u).Where(u => userGroups.All(ug => u.Id != ug.UserId)));
                foreach (var newMember in newMembers)
                {
                    performContext?.Cancel();

                    await PortalService.AddUserGroupAsync(
                        LicenseUserGroup.Create(@group.Id, newMember.Id));

                    performContext?.WriteSuccessLine($"+ {group.Name}  {newMember.DisplayName}");
                    Logger.Info($"User: {newMember.Id} was added to Group: {group.Id} {group.Name}");
                }

                var staleMembers = new HashSet<LicenseUserGroup>(userGroups.Where(ug => groupMembers.Users.All(u => ug.UserId != u.Id)));
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
            var adGroups = _activeDirectoryManager
                .GetAllGroupsList(performContext)
                .OrderBy(g => g.Name)
                .ToList();

            var remoteGroups = await PortalService.GetAllGroupsAsync();

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

            var staleGroups = new HashSet<LicenseGroup>(remoteGroups.Where(ru => adGroups.All(au => au.Id != ru.Id)));
            foreach (var staleGroup in staleGroups)
            {
                performContext?.Cancel();

                await PortalService.DeleteGroupAsync(staleGroup);

                performContext?.WriteWarnLine($"- {staleGroup}");
                Logger.Info($"Delete: {staleGroup}");
                Logger.Debug($"{JsonConvert.SerializeObject(staleGroup, Formatting.Indented)}");
            }
        }
    }
}