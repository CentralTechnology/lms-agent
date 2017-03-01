namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using Models;
    using ShellProgressBar;
    using Simple.OData.Client;
    using Extensions;

    public class LicenseUserClient : PortalLicenseClient, ILicenseUserClient
    {
        private readonly PortalLicenseClient _client;

        public LicenseUserClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public async Task Add(List<LicenseUser> users, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("adding users");

            using (var pbar = childProgressBar.Spawn(users.Count, "adding users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];
                   
                    try
                    {
                        await _client.For<LicenseUser>().Set(new
                        {
                            user.DisplayName,
                            user.Email,
                            user.Enabled,
                            user.FirstName,
                            user.Id,
                            user.SupportUploadId,
                            user.Surname,
                            user.WhenCreated
                        }).InsertEntryAsync();

                        pbar.Tick($"adding: {user.DisplayName}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {user.DisplayName}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar.Tick();
        }

        public async Task Remove(List<LicenseUser> users, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("removing users");

            using (var pbar = childProgressBar.Spawn(users.Count, "removing users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];
                    
                    try
                    {
                        await _client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync();

                        pbar.Tick($"removing: {user.DisplayName}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to delete: {user.DisplayName}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar.Tick();
        }

        public async Task Update(List<LicenseUser> users, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("updating users");

            using (var pbar = childProgressBar.Spawn(users.Count, "updating users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];
                  
                    try
                    {
                        await _client.For<LicenseUser>().Key(user.Id).Set(new
                        {
                            user.DisplayName,
                            user.Email,
                            user.Enabled,
                            user.FirstName,
                            user.Surname,
                            user.WhenCreated
                        }).UpdateEntryAsync();

                        pbar.Tick($"updating: {user.DisplayName}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to update: {user.DisplayName}");
                        Logger.Error($"Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar.Tick();
        }

        public async Task<List<LicenseUser>> GetAll()
        {
            try
            {
                var users = await _client.For<LicenseUser>().Expand(u => u.Groups).FindEntriesAsync();
                return users.ToList();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of users from the api.");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }
    }

    public class SupportUploadClient : PortalLicenseClient, ISupportUploadClient
    {
        private readonly PortalLicenseClient _client;

        public SupportUploadClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public async Task<CallInStatus> GetStatusByDeviceId(Guid deviceId)
        {
            try
            {
                return await _client.For<SupportUpload>().Function("GetCallInStatus").Set(new {deviceId}).ExecuteAsScalarAsync<CallInStatus>();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return CallInStatus.NotCalledIn;
            }
            catch (Exception ex)
            {
                Logger.Error($"Status: {ex.Message}");
                Logger.Error($"Failed to get the call in status for device: {deviceId}");
                Logger.DebugFormat("Exception: ", ex);

                // by default return not called in, its not the end of the world if they call in twice
                return CallInStatus.NotCalledIn;
            }
        }

        public async Task<int> GetUploadIdByDeviceId(Guid deviceId)
        {
            try
            {
                return await _client.For<SupportUpload>().Function("GetUploadId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the upload id for device: {deviceId}");
                Logger.DebugFormat("Exception: ", ex);

                // default return from the api
                return 0;
            }
        }

        public async Task Update(int id)
        {
            var uploadId = await GetNewUploadId();

            try
            {
                await _client.For<SupportUpload>().Key(id).Set(new
                {
                    CheckInTime = DateTime.Now,
                    Hostname = Environment.MachineName,
                    Status = CallInStatus.CalledIn,
                    UploadId = uploadId
                }).UpdateEntryAsync();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to update upload: {id}");
                Logger.DebugFormat("Exception: ", ex);
            }
        }

        public async Task<SupportUpload> Add(SupportUpload upload)
        {
            try
            {
                return await _client.For<SupportUpload>().Set(upload).InsertEntryAsync();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to add upload");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }

        public async Task<SupportUpload> Get(int id)
        {
            try
            {
                var upload = await _client.For<SupportUpload>().Key(id).Expand(s => s.Users).FindEntryAsync();
                return upload;
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to find upload: {id}");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }

        public async Task<List<LicenseUser>> GetUsers(int uploadId)
        {
            try
            {
                var upload = await _client.For<SupportUpload>()
                    .Key(uploadId)
                    .Expand(x => x.Users)
                    .FindEntryAsync();

                // return a new list if null, could just be the first check in
                return upload.Users ?? new List<LicenseUser>();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the users for upload: {uploadId}");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }

        public async Task<int> GetNewUploadId()
        {
            try
            {
                return await _client.For<SupportUpload>().Function("NewUploadId").ExecuteAsScalarAsync<int>();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to get a new upload id");
                Logger.DebugFormat("Exception: ", ex);

                // default return from the api
                return 0;
            }
        }
    }

    public class ProfileClient : PortalLicenseClient, IProfileClient
    {
        private readonly PortalLicenseClient _client;

        public ProfileClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public async Task<int> GetAccountByDeviceId(Guid deviceId)
        {
            try
            {
                return await _client.Unbound().Function("GetAccountId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the account id for device: {deviceId}");
                Logger.DebugFormat("Exception: ", ex);

                throw;
            }
        }
    }

    public class LicenseGroupClient : PortalLicenseClient, ILicenseGroupClient
    {
        private readonly PortalLicenseClient _client;

        public LicenseGroupClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public async Task Add(List<LicenseGroup> groups, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("adding groups");

            using (var pbar = childProgressBar.Spawn(groups.Count, "adding groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];
                   
                    try
                    {
                        await _client.For<LicenseGroup>().Set(new
                        {
                            group.Id,
                            group.Name,
                            group.WhenCreated
                        }).InsertEntryAsync();

                        pbar.Tick($"adding: {group.Name}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {group.Name}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar.Tick();
        }

        public async Task Remove(List<LicenseGroup> groups, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("removing groups");

            using (var pbar = childProgressBar.Spawn(groups.Count, "removing groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];
                   
                    try
                    {
                        await _client.For<LicenseGroup>().Key(group.Id).DeleteEntryAsync();

                        pbar.Tick($"removing: {group.Name}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to delete: {group.Name}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar.Tick();
        }

        public async Task Update(List<LicenseGroup> groups, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("updating groups");

            using (var pbar = childProgressBar.Spawn(groups.Count, "updating groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];

                    try
                    {
                        await _client.For<LicenseGroup>().Key(group.Id).Set(new
                        {
                            group.Name,
                            group.WhenCreated
                        }).UpdateEntryAsync();

                        pbar.Tick($"updating: {group.Name}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to update: {group.Name}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar.Tick();
        }

        public async Task<List<LicenseGroup>> GetAll()
        {
            try
            {
                var groups = await _client.For<LicenseGroup>().FindEntriesAsync();
                return groups.ToList();
            }
            catch (WebRequestException ex)
            {
                FormatWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of groups from the api.");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }
    }

    public class LicenseUserGroupClient : PortalLicenseClient, ILicenseUserGroupClient
    {
        private readonly PortalLicenseClient _client;

        public LicenseUserGroupClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public async Task Add(List<LicenseUser> users, LicenseGroup group, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("updating group membership");

            using (var pbar = childProgressBar.Spawn(users.Count, $"adding users to group: {group.Name}", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    try
                    {
                        await _client.For<LicenseUser>().Key(user.Id).LinkEntryAsync(group,"Groups");

                        pbar.Tick($"adding: {user.DisplayName}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {user.DisplayName}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }
        }

        public async Task Remove(List<LicenseUser> users, LicenseGroup group, ChildProgressBar childProgressBar)
        {
            childProgressBar.UpdateMessage("updating group membership");

            using (var pbar = childProgressBar.Spawn(users.Count, $"removing users from group: {group.Name}", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            }))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    try
                    {
                        await _client.For<LicenseUser>().Key(user.Id).UnlinkEntryAsync(group, "Groups");

                        pbar.Tick($"removing: {user.DisplayName}", LoggerLevel.Debug);
                    }
                    catch (WebRequestException ex)
                    {
                        FormatWebRequestException(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {user.DisplayName}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }
        }
    }
}