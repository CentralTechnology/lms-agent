namespace Core.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Abp.Timing;
    using Administration;
    using Common.Client;
    using Common.Extensions;
    using Factory;
    using MarkdownLog;
    using Models;
    using NLog;

    public class UserOrchestrator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<ManagedSupport> ProcessUpload()
        {
            Logger.Info("Processing Upload Information".SectionTitle());

            Guid deviceId = SettingFactory.SettingsManager().Read().DeviceId;
            Logger.Debug($"Device id thats registered in settings: {deviceId}");

            Logger.Debug("Obtaining the upload id.");
            int uploadId = await ClientFactory.SupportUploadClient().GetUploadIdByDeviceId(deviceId);
            Logger.Debug($"Upload Id: {uploadId}");

            Logger.Info("Getting the call in status.");
            ManagedSupport upload;
            if (uploadId != 0)
            {
                upload = await ClientFactory.SupportUploadClient().Get(uploadId);

                Logger.Info($"Status: {upload.Status}\t Last Check In: {upload.CheckInTime.ToUniversalTime()}");

                return upload;
            }

            Logger.Warn("This is the first time this device has called in. A new upload request is needed.");
            Logger.Info("Creating a new upload.");

            int updateId = await ClientFactory.SupportUploadClient().GetNewUploadId();

            upload = await ClientFactory.SupportUploadClient().Add(new ManagedSupport
            {
                CheckInTime = Clock.Now,
                DeviceId = deviceId,
                Hostname = Environment.MachineName,
                IsActive = true,
                Status = CallInStatus.NotCalledIn,
                UploadId = updateId
            });

            if (upload == null)
            {
                return null;
            }

            Logger.Info($"A new upload has been created with id: {upload.Id}");
            return upload;
        }

        public async Task<List<LicenseUser>> ProcessUsers(int uploadId)
        {
            Logger.Info("Processing User Information".SectionTitle());

            // get the local users 
            Logger.Info("Getting a list of local users from Active Directory.");
            List<LicenseUser> localUsers = UserFactory.UserManager().GetUsersAndGroups();
            Logger.Info($"{localUsers.Count} local users have been found in the Active Directory.");

            // get the api users
            Logger.Info("Getting a list of users that have already been created in the api.");
            List<LicenseUser> remoteUsers = await ClientFactory.SupportUploadClient().GetUsers(uploadId);
            Logger.Info($"{remoteUsers.Count} users have been returned from the api.");

            // return a list of users that need adding to the api
            Logger.Info("Calculating the number of users that need to be created in the api.");
            List<LicenseUser> usersToCreate = remoteUsers.FilterCreate<LicenseUser, Guid>(localUsers);
            Logger.Info($"{usersToCreate.Count} users need creating in the api.");

            if (usersToCreate.Count > 0)
            {
                Logger.Info($"Applying the upload id: {uploadId} to all the local users.");
                usersToCreate = usersToCreate.ApplyUploadId(uploadId);

                Logger.Info("Creating the users.");
                await ClientFactory.LicenseUserClient().Add(usersToCreate);
            }

            Logger.Info("Calculating the number of users that need to be updated in the api.");
            List<LicenseUser> usersToUpdate = remoteUsers.FilterUpdate<LicenseUser, Guid>(localUsers);
            Logger.Info($"{usersToUpdate.Count} users need updating in the api.");

            if (usersToUpdate.Count > 0)
            {
                Logger.Info("Updating the users.");
                await ClientFactory.LicenseUserClient().Update(usersToUpdate);
            }

            Logger.Info("Calculating the number of users that need to be deleted in the api.");
            List<LicenseUser> usersToDelete = remoteUsers.FilterDelete<LicenseUser, Guid>(localUsers);
            Logger.Info($"{usersToDelete.Count} users need deleting in the api.");

            if (usersToDelete.Count > 0)
            {
                Logger.Info("Deleting the users.");
                await ClientFactory.LicenseUserClient().Remove(usersToDelete);
            }

            var summary = new[]
            {
                new {Action = "Created", usersToCreate.Count},
                new {Action = "Delete", usersToDelete.Count},
                new {Action = "Update", usersToUpdate.Count}
            };

            Logger.Info($"{Environment.NewLine}{summary.ToMarkdownTable()}");
            return localUsers;
        }

        public async Task ProcessGroups(List<LicenseUser> users)
        {
            Logger.Info("Processing Group Information".SectionTitle());

            // get the local groups
            Logger.Info("Getting a list of local groups from Active Directory.");
            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

            // get the api groups
            Logger.Info("Getting a list of groups that have already been created in the api.");
            List<LicenseGroup> remoteGroups = await ClientFactory.LicenseGroupClient().GetAll();
            Logger.Info($"{remoteGroups.Count} groups have been returned from the api.");

            // return a list of groups that need adding to the api
            Logger.Info("Calculating the number of groups that need to be created in the api.");
            List<LicenseGroup> groupsToCreate = remoteGroups.FilterCreate<LicenseGroup, Guid>(localGroups);
            Logger.Info($"{groupsToCreate.Count} groups need creating in the api.");

            if (groupsToCreate.Count > 0)
            {
                Logger.Info("Ccreating the groups.");
                await ClientFactory.LicenseGroupClient().Add(groupsToCreate);
            }

            Logger.Info("Calculating the number of groups that need to be updated in the api.");
            List<LicenseGroup> groupsToUpdate = remoteGroups.FilterUpdate<LicenseGroup, Guid>(localGroups);
            Logger.Info($"{groupsToUpdate.Count} groups need updating in the api.");

            if (groupsToUpdate.Count > 0)
            {
                Logger.Info("Updating the groups.");
                await ClientFactory.LicenseGroupClient().Update(groupsToUpdate);
            }

            Logger.Info("Calculating the number of groups that need to be deleted in the api.");
            List<LicenseGroup> groupsToDelete = remoteGroups.FilterDelete<LicenseGroup, Guid>(localGroups);
            Logger.Info($"{groupsToDelete.Count} groups need deleting in the api.");

            if (groupsToDelete.Count > 0)
            {
                Logger.Info("Deleting the groups.");
                await ClientFactory.LicenseGroupClient().Remove(groupsToDelete);
            }

            var summary = new[]
            {
                new {Action = "Created", groupsToCreate.Count},
                new {Action = "Delete", groupsToDelete.Count},
                new {Action = "Update", groupsToUpdate.Count}
            };

            Logger.Info($"{Environment.NewLine}{summary.ToMarkdownTable()}");
        }

        public async Task ProcessUserGroups(List<LicenseUser> users)
        {
            Logger.Info("Processing User Group Information".SectionTitle());

            // get the local groups
            Logger.Info("Getting a list of local groups from Active Directory.");
            List<LicenseGroup> localGroups = users.SelectMany(u => u.Groups).Distinct().ToList();

            // get the remote users and groups
            Logger.Info("Getting a list of users and their group membership from the api.");
            List<LicenseUser> apiUsers = await ClientFactory.LicenseUserClient().GetAll();

            // if groups are null then create a new list of groups
            List<LicenseUser> remoteUsers = apiUsers.Select(u =>
            {
                u.Groups = u.Groups ?? new List<LicenseGroup>();
                return u;
            }).ToList();

            foreach (LicenseGroup localGroup in localGroups)
            {
                Logger.Debug($"Processing group: {localGroup}");

                List<LicenseUser> usersThatWereMembers = remoteUsers.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();
                List<LicenseUser> usersThatAreMembers = users.Where(u => u.Groups.Any(g => g.Id.Equals(localGroup.Id))).ToList();

                Logger.Debug("Calculating the number of users that need to be added to this group.");
                List<LicenseUser> usersToBeAdded = usersThatWereMembers.FilterCreate<LicenseUser, Guid>(usersThatAreMembers);
                Logger.Debug($"Adding {usersToBeAdded.Count} users.");
                if (usersToBeAdded.Count > 0)
                {
                    await ClientFactory.LicenseUserGroupClient().Add(usersToBeAdded, localGroup);
                }

                Logger.Debug("Calculating the number of users that need to be removed from this group.");
                List<LicenseUser> usersToBeRemoved = usersThatWereMembers.FilterDelete<LicenseUser, Guid>(usersThatAreMembers);
                Logger.Debug($"Removing {usersToBeAdded.Count} users.");
                if (usersToBeRemoved.Count > 0)
                {
                    await ClientFactory.LicenseUserGroupClient().Remove(usersToBeRemoved, localGroup);
                }
            }
        }

        public async Task CallIn(int uploadId)
        {
            Logger.Info("Processing Upload Information".SectionTitle());
            Logger.Info("Calling in");

            await ClientFactory.SupportUploadClient().Update(uploadId);
        }
    }
}