namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Administration;
    using Common.Client;
    using Common.Extensions;
    using Models;
    using ShellProgressBar;

    public class UserOrchestrator : DomainService, IUserOrchestrator
    {
        private readonly ILicenseGroupClient _licenseGroupClient;
        private readonly ILicenseUserClient _licenseUserClient;
        private readonly ILicenseUserGroupClient _licenseUserGroupClient;
        private readonly ISupportUploadClient _uploadClient;
        private readonly IUserManager _userManager;
        private readonly ISettingsManager _settingsManager;

        public UserOrchestrator(
            ISupportUploadClient uploadClient,
            ILicenseUserClient licenseUserClient,
            ILicenseGroupClient licenseGroupClient,
            ILicenseUserGroupClient licenseUserGroupClient,
            IUserManager userManager,
            ISettingsManager settingsManager)
        {
            _uploadClient = uploadClient;
            _licenseUserClient = licenseUserClient;
            _licenseGroupClient = licenseGroupClient;
            _licenseUserGroupClient = licenseUserGroupClient;
            _userManager = userManager;
            _settingsManager = settingsManager;
        }

        public async Task<int> ProcessUpload(ProgressBar pbar)
        {
            int initialProgress = 2;
            using (var childProgress = Environment.UserInteractive && pbar != null ? pbar.Spawn(initialProgress, "obtaining device information", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false,
            }) : null)
            {
                var deviceId = _settingsManager.Read().DeviceId;
                childProgress?.Tick($"device id: {deviceId}");

                var status = await _uploadClient.GetStatusByDeviceId(deviceId);
                Logger.Debug($"Current status: {status}");

                switch (status)
                {
                    case CallInStatus.CalledIn:
                        childProgress?.Tick();
                        pbar?.Tick("this device is called in. nothing to process.");
                        return 0;

                    case CallInStatus.NotCalledIn:
                        initialProgress++;
                        childProgress?.UpdateMaxTicks(initialProgress);
                        childProgress?.Tick();
                        pbar?.UpdateMessage("this device is not called in.");


                        var id = await _uploadClient.GetUploadIdByDeviceId(deviceId);
                        childProgress?.Tick($"upload id: {id}");
                        pbar?.Tick();
                        return id;

                    case CallInStatus.NeverCalledIn:
                        initialProgress++;
                        childProgress?.UpdateMaxTicks(initialProgress);
                        childProgress?.Tick();
                        pbar?.UpdateMessage("this device has never called in.");
                        Logger.Debug($"this device has never called in, therefore a new upload must be created");

                        await Task.Delay(1000);
                        pbar?.UpdateMessage("attemting to call in...");
                        Logger.Debug($"attempting to get a new upload id");

                        var uploadId = await _uploadClient.GetNewUploadId();

                        Logger.Debug($"upload id return was {uploadId}");

                        if (uploadId == 0)
                        {
                            Logger.Debug("upload id was 0 therefore the call in failed.");
                            return 0;
                        }

                        Logger.Debug("attempting to create a new upload");

                        var upload = await _uploadClient.Add(new SupportUpload
                        {
                            CheckInTime = DateTime.Now,
                            DeviceId = deviceId,
                            Hostname = Environment.MachineName,
                            IsActive = true,
                            Status = CallInStatus.CalledIn,
                            UploadId = uploadId
                        });

                        if (upload == null)
                        {
                            pbar?.Tick("call in was unsuccessful");
                            Logger.Debug("call in was unsuccessful");
                            return 0;
                        }

                        childProgress?.Tick($"call in was successfull: {upload.Id}");
                        Logger.Debug("the device has successfully called in with the portal.");
                        Logger.Debug($"the upload id returned was {upload.Id} ");

                        pbar?.Tick();
                        return upload.Id;
                }
            }

            // should never hit this point as there are return statements in the switch statement
            // this could possibly be refactored??
            return 0;
        }

        public async Task<List<LicenseUser>> ProcessUsers(int uploadId, ProgressBar pbar)
        {
            // progress bar configuration
            var initialProgress = 3;
            using (var childProgress = Environment.UserInteractive && pbar != null ? pbar.Spawn(initialProgress, "processing users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false,
            }) : null)
            {
                // get the local users 
                childProgress?.UpdateMessage("getting local users.");
                Logger.Debug("getting local users.");

                var localUsers = _userManager.GetUsersAndGroups(childProgress);
               
                // update progress bar
                childProgress?.UpdateMessage($"local users found: {localUsers.Count}.");
                Logger.Debug($"local users found: {localUsers.Count}.");

                // get the api users
                childProgress?.UpdateMessage("getting api users.");
                Logger.Debug("getting api users");

                var remoteUsers = await _uploadClient.GetUsers(uploadId);
                
                // update progress bar
                childProgress?.Tick($"api users found: {remoteUsers.Count}.");
                Logger.Debug($"api users found: {remoteUsers.Count}.");

                // return a list of users that need adding to the api
                var usersToCreate = remoteUsers.FilterCreate<LicenseUser, Guid>(localUsers);
                childProgress?.UpdateMessage($"users that need adding: {usersToCreate.Count}.");
                Logger.Debug($"users that need adding: {usersToCreate.Count}.");

                if (usersToCreate.Count > 0)
                {
                    // update progress bar with new tick count
                    initialProgress++;
                    childProgress?.UpdateMaxTicks(initialProgress);

                    usersToCreate = usersToCreate.ApplyUploadId(uploadId);
                    childProgress?.UpdateMessage("Applying the upload id to the users.");

                    childProgress?.UpdateMessage("adding users.");
                    await _licenseUserClient.Add(usersToCreate, childProgress);
                }

                var usersToUpdate = remoteUsers.FilterUpdate<LicenseUser, Guid>(localUsers);
                childProgress?.UpdateMessage($"users that need updating: {usersToUpdate.Count}.");

                if (usersToUpdate.Count > 0)
                {
                    // update progress bar with new tick count
                    initialProgress++;
                    childProgress?.UpdateMaxTicks(initialProgress);

                    childProgress?.UpdateMessage("updating users.");
                    await _licenseUserClient.Update(usersToUpdate, childProgress);
                }

                var usersToDelete = remoteUsers.FilterDelete<LicenseUser, Guid>(localUsers);
                childProgress?.UpdateMessage($"users that need removing: {usersToDelete.Count}.");

                if (usersToDelete.Count > 0)
                {
                    // update progress bar with new tick count
                    initialProgress++;
                    childProgress?.UpdateMaxTicks(initialProgress);

                    childProgress?.UpdateMessage("removing users.");
                    await _licenseUserClient.Remove(usersToDelete, childProgress);
                }

                childProgress?.Tick($"users created: {usersToCreate.Count}\t users updated: {usersToUpdate.Count} \t users removed: {usersToDelete.Count}");
                pbar?.Tick();
                return localUsers;
            }
        }

        public async Task ProcessGroups(List<LicenseUser> users, ProgressBar pbar)
        {
            // progress bar configuration
            var initialProgress = 2;
            using (var childProgress = Environment.UserInteractive && pbar != null ? pbar.Spawn(initialProgress, "processing groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false
            }) : null)
            {
                // get the local groups
                childProgress?.UpdateMessage("getting local groups");
                var localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

                // update progress bar
                childProgress?.UpdateMessage($"local groups found: {localGroups.Count}.");

                // get the api groups
                childProgress?.UpdateMessage("getting api groups.");
                var remoteGroups = await _licenseGroupClient.GetAll();

                // update progress bar
                childProgress?.Tick($"api groups found: {remoteGroups.Count}.");

                // return a list of groups that need adding to the api
                var groupsToCreate = remoteGroups.FilterCreate<LicenseGroup, Guid>(localGroups);
                childProgress?.UpdateMessage($"groups that need adding: {groupsToCreate.Count}.");

                if (groupsToCreate.Count > 0)
                {
                    // update progress bar with new tick count
                    initialProgress++;
                    childProgress?.UpdateMaxTicks(initialProgress);

                    childProgress?.UpdateMessage("adding groups.");
                    await _licenseGroupClient.Add(groupsToCreate, childProgress);
                }

                var groupsToUpdate = remoteGroups.FilterUpdate<LicenseGroup, Guid>(localGroups);
                childProgress?.UpdateMessage($"groups that need updating: {groupsToUpdate.Count}.");

                if (groupsToUpdate.Count > 0)
                {
                    // update progress bar with new tick count
                    initialProgress++;
                    childProgress?.UpdateMaxTicks(initialProgress);

                    childProgress?.UpdateMessage("updating groups.");

                    await _licenseGroupClient.Update(groupsToUpdate, childProgress);
                }

                var groupsToDelete = remoteGroups.FilterDelete<LicenseGroup, Guid>(localGroups);
                childProgress?.UpdateMessage($"groups that need removing: {groupsToDelete.Count}.");

                if (groupsToDelete.Count > 0)
                {
                    // update progress bar with new tick count
                    initialProgress++;
                    childProgress?.UpdateMaxTicks(initialProgress);

                    childProgress?.UpdateMessage("removing groups.");

                    await _licenseGroupClient.Remove(groupsToDelete, childProgress);
                }

                childProgress?.Tick($"groups created: {groupsToCreate.Count}\t groups updated: {groupsToUpdate.Count} \t groups removed: {groupsToDelete.Count}");
            }

            pbar?.Tick();
        }

        public async Task ProcessUserGroups(List<LicenseUser> users, ProgressBar pbar)
        {
            // progress bar configuration
            var initialProgress = 2;
            using (var childProgress = Environment.UserInteractive && pbar != null ? pbar.Spawn(initialProgress, "processing user groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false,
            }) : null)
            {
                // get the local groups
                childProgress?.UpdateMessage("getting local groups.");
                var localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

                // update progress bar
                childProgress?.UpdateMessage($"local groups found: {localGroups.Count}.");

                // get the remote users and groups
                childProgress?.UpdateMessage("getting api users with groups.");
                var apiUsers = await _licenseUserClient.GetAll();

                // if groups are null then create a new list of groups
                var remoteUsers = apiUsers.Select(u =>
                {
                    u.Groups = u.Groups ?? new List<LicenseGroup>();
                    return u;
                }).ToList();

                // update progress bar
                childProgress?.Tick($"api users and groups found: {remoteUsers.Count}.");
                childProgress?.UpdateMaxTicks(initialProgress + localGroups.Count);
                foreach (var localGroup in localGroups)
                {
                    // update progress bar
                    childProgress?.UpdateMessage($"processing: {localGroup.Name}");

                    var usersThatWereMembers = remoteUsers.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();
                    var usersThatAreMembers = users.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();

                    var usersToBeAdded = usersThatWereMembers.FilterCreate<LicenseUser, Guid>(usersThatAreMembers);
                    childProgress?.UpdateMessage($"{usersToBeAdded.Count} users need adding to the group {localGroup.Name}");
                    if (usersToBeAdded.Count > 0)
                    {
                        await _licenseUserGroupClient.Add(usersToBeAdded, localGroup, childProgress);
                    }

                    var usersToBeRemoved = usersThatWereMembers.FilterDelete<LicenseUser, Guid>(usersThatAreMembers);
                    childProgress?.UpdateMessage($"{usersToBeRemoved.Count} users need removing from the group {localGroup.Name}");
                    if (usersToBeRemoved.Count > 0)
                    {
                        await _licenseUserGroupClient.Remove(usersToBeRemoved, localGroup, childProgress);
                    }
                    childProgress?.Tick();
                }

                childProgress?.Tick();
            }

            pbar?.Tick();
        }

        public async Task CallIn(int uploadId, ProgressBar pbar)
        {
            await _uploadClient.Update(uploadId);
            pbar?.Tick("call in complete.");
        }
    }
}