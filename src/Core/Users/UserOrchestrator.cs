namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using Common.Client;
    using Common.Extensions;
    using Models;
    using ShellProgressBar;

    public class UserOrchestrator : LicenseMonitoringBase, IUserOrchestrator
    {
        private readonly ILicenseGroupClient _licenseGroupClient;
        private readonly ILicenseUserClient _licenseUserClient;
        private readonly ILicenseUserGroupClient _licenseUserGroupClient;
        private readonly ISupportUploadClient _uploadClient;
        private readonly IUserManager _userManager;

        public UserOrchestrator(
            ISupportUploadClient uploadClient,
            ILicenseUserClient licenseUserClient,
            ILicenseGroupClient licenseGroupClient,
            ILicenseUserGroupClient licenseUserGroupClient,
            IUserManager userManager)
        {
            _uploadClient = uploadClient;
            _licenseUserClient = licenseUserClient;
            _licenseGroupClient = licenseGroupClient;
            _licenseUserGroupClient = licenseUserGroupClient;
            _userManager = userManager;
        }

        public async Task<int> ProcessUpload()
        {
            var deviceId = SettingManager.GetDeviceId();

            var status = await _uploadClient.GetStatusByDeviceId(deviceId);

            switch (status)
            {
                case CallInStatus.CalledIn:

                    ConsoleExtensions.WriteLineBottom("You are currently called in. Nothing to process", LoggerLevel.Info);
                    return 0;

                case CallInStatus.NotCalledIn:

                    ConsoleExtensions.WriteLineBottom("You are not currently called in.", LoggerLevel.Error);
                    break;

                case CallInStatus.NeverCalledIn:

                    ConsoleExtensions.WriteLineBottom("You have never called in", LoggerLevel.Warn);
                    ConsoleExtensions.WriteLineBottom("Calling in...", LoggerLevel.Info);

                    var uploadId = await _uploadClient.GetNewUploadId();

                    if (uploadId == 0)
                    {
                        break;
                    }

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
                        ConsoleExtensions.WriteLineBottom("Failed to call in.", LoggerLevel.Error);
                        break;
                    }

                    ConsoleExtensions.WriteLineBottom($"Call in successfull: {upload.Id}", LoggerLevel.Info);
                    return upload.Id;
            }

            var id = await _uploadClient.GetUploadIdByDeviceId(deviceId);
            ConsoleExtensions.WriteLineBottom($"Upload id: {id}", LoggerLevel.Debug);
            return id;
        }

        public async Task<List<LicenseUser>> ProcessUsers(int uploadId, ProgressBar pbar)
        {
            // progress bar configuration
            var operationCount = 3;
            using (var childProgress = pbar.Spawn(operationCount, "processing users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false,
            }))
            {
                // get the local users 
                Console.WriteLine("getting local users.");
                //childProgress.UpdateMessage("getting local users.", LoggerLevel.Info);
                var localUsers = _userManager.GetUsersAndGroups(childProgress);

                // update progress bar
                Console.WriteLine($"local users found: {localUsers.Count}.");
              //  childProgress.UpdateMessage($"local users found: {localUsers.Count}.", LoggerLevel.Info);

                // get the api users
                Console.WriteLine("getting api users.");
              //  childProgress.UpdateMessage("getting api users.", LoggerLevel.Info);
                var remoteUsers = await _uploadClient.GetUsers(uploadId);

                // update progress bar
                childProgress.Tick($"api users found: {remoteUsers.Count}.", LoggerLevel.Info);

                // return a list of users that need adding to the api
                var usersToCreate = remoteUsers.FilterCreate<LicenseUser, Guid>(localUsers);
                ConsoleExtensions.WriteLineBottom($"users that need adding: {usersToCreate.Count}.", LoggerLevel.Debug);

                if (usersToCreate.Count > 0)
                {
                    // update progress bar with new tick count
                    operationCount++;
                    childProgress.UpdateMaxTicks(operationCount);

                    usersToCreate = usersToCreate.ApplyUploadId(uploadId);
                    ConsoleExtensions.WriteLineBottom("Applying the upload id to the users.", LoggerLevel.Debug);

                    childProgress.UpdateMessage("adding users.");
                    await _licenseUserClient.Add(usersToCreate, childProgress);
                }

                var usersToUpdate = remoteUsers.FilterUpdate<LicenseUser, Guid>(localUsers);
                ConsoleExtensions.WriteLineBottom($"users that need updating: {usersToUpdate.Count}.", LoggerLevel.Debug);

                if (usersToUpdate.Count > 0)
                {
                    // update progress bar with new tick count
                    operationCount++;
                    childProgress.UpdateMaxTicks(operationCount);

                    childProgress.UpdateMessage("updating users.", LoggerLevel.Info);
                    await _licenseUserClient.Update(usersToUpdate, childProgress);
                }

                var usersToDelete = remoteUsers.FilterDelete<LicenseUser, Guid>(localUsers);
                ConsoleExtensions.WriteLineBottom($"users that need removing: {usersToDelete.Count}.", LoggerLevel.Debug);

                if (usersToDelete.Count > 0)
                {
                    // update progress bar with new tick count
                    operationCount++;
                    childProgress.UpdateMaxTicks(operationCount);

                    childProgress.UpdateMessage("removing users.");
                    await _licenseUserClient.Remove(usersToDelete, childProgress);
                }

                childProgress.Tick();
                pbar.Tick();
                return localUsers;
            }
        }

        public async Task ProcessGroups(List<LicenseUser> users, ProgressBar pbar)
        {
            // progress bar configuration
            var operationCount = 3;
            using (var childProgress = pbar.Spawn(operationCount, "processing groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false
            }))
            {
                // get the local groups
                childProgress.UpdateMessage("getting local groups");
                var localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

                // update progress bar
                childProgress.Tick($"local groups found: {localGroups.Count}.");
                Logger.Info($"local groups found: {localGroups.Count}.");

                // get the remote groups
                childProgress.UpdateMessage("getting api groups.");
                var remoteGroups = await _licenseGroupClient.GetAll();

                // update progress bar
                childProgress.Tick($"api groups found: {remoteGroups.Count}.");
                Logger.Info($"api groups found: {remoteGroups.Count}.");

                // return a list of groups that need adding to the api
                var groupsToCreate = remoteGroups.FilterCreate<LicenseGroup, Guid>(localGroups);
                Logger.Info($"groups that need adding: {groupsToCreate.Count}.");

                if (groupsToCreate.Count > 0)
                {
                    // update progress bar with new tick count
                    operationCount++;
                    childProgress.UpdateMaxTicks(operationCount);

                    await _licenseGroupClient.Add(groupsToCreate, childProgress);
                }

                var groupsToUpdate = remoteGroups.FilterUpdate<LicenseGroup, Guid>(localGroups);
                Logger.Info($"{groupsToUpdate.Count} groups need updating.");
                if (groupsToUpdate.Count > 0)
                {
                    await _licenseGroupClient.Update(groupsToUpdate, childProgress);
                }

                var groupsToDelete = remoteGroups.FilterDelete<LicenseGroup, Guid>(localGroups);
                Logger.Info($"{groupsToDelete.Count} groups need removing.");
                if (groupsToDelete.Count > 0)
                {
                    await _licenseGroupClient.Remove(groupsToDelete, childProgress);
                }

                childProgress.Tick();
            }

            pbar.Tick();
        }

        public async Task ProcessUserGroups(List<LicenseUser> users, ProgressBar pbar)
        {
            // progress bar configuration
            var operationCount = 3;
            using (var childProgress = pbar.Spawn(operationCount, "user groups progress", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Cyan,
                ForeGroundColorDone = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
                CollapseWhenFinished = false,
            }))
            {
                // get the local groups
                childProgress.UpdateMessage("getting local group definitions.");
                var localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

                // update progress bar
                childProgress.Tick($"local group definitions found: {localGroups.Count}.");
                Logger.Info($"local group defintions found: {localGroups.Count}.");

                // get the remote users and groups
                childProgress.UpdateMessage("getting api users with groups.");
                var apiUsers = await _licenseUserClient.GetAll();

                // if groups are null then create a new list of groups
                var remoteUsers = apiUsers.Select(u =>
                {
                    u.Groups = u.Groups ?? new List<LicenseGroup>();
                    return u;
                }).ToList();

                childProgress.Tick($"api users and groups found: {remoteUsers.Count}.");
                Logger.Info($"api users and groups found: {remoteUsers.Count}.");

                using (var innerChildProgress = childProgress.Spawn(localGroups.Count, "processing group membership", new ProgressBarOptions
                {
                    ForeGroundColor = ConsoleColor.Magenta,
                    ProgressCharacter = '-',
                    BackgroundColor = ConsoleColor.DarkGray
                }))
                {
                    foreach (var localGroup in localGroups)
                    {
                        // update progress bar
                        childProgress.UpdateMessage("processing group membership");
                        innerChildProgress.UpdateMessage($"processing: {localGroup.Name}");
                        Logger.Info($"processing: {localGroup.Name}");

                        var usersThatWereMembers = remoteUsers.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();
                        var usersThatAreMembers = users.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();

                        var usersToBeAdded = usersThatWereMembers.FilterCreate<LicenseUser, Guid>(usersThatAreMembers);
                        Logger.Info($"{usersToBeAdded.Count} users need adding to the group {localGroup.Name}");
                        if (usersToBeAdded.Count > 0)
                        {
                            await _licenseUserGroupClient.Add(usersToBeAdded, localGroup, innerChildProgress);
                        }

                        var usersToBeRemoved = usersThatWereMembers.FilterDelete<LicenseUser, Guid>(usersThatAreMembers);
                        Logger.Info($"{usersToBeRemoved.Count} users need removing from the group {localGroup.Name}");
                        if (usersToBeRemoved.Count > 0)
                        {
                            await _licenseUserGroupClient.Remove(usersToBeRemoved, localGroup, innerChildProgress);
                        }

                        innerChildProgress.Tick();
                    }
                }

                childProgress.Tick();
            }

            pbar.Tick();
        }

        public async Task CallIn(int uploadId)
        {
            await _uploadClient.Update(uploadId);

            Logger.Info($"You are now called in.");
        }
    }
}