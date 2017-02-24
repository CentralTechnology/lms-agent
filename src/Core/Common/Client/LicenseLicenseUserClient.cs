namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;
    using ShellProgressBar;

    public class LicenseLicenseUserClient : LicenseMonitoringBase, ILicenseUserClient
    {
        private readonly PortalLicenseClient _client;

        public LicenseLicenseUserClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public async Task Add(List<LicenseUser> users)
        {
            using (var progressBar = new ProgressBar(users.Count, "Adding users", ConsoleColor.White))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    progressBar.Tick($"Processing: {user.DisplayName} \t #{index}");

                    try
                    {
                        await _client.For<LicenseUser>().Set(user).InsertEntryAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {user.DisplayName}");
                        Logger.Error($"Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }
        }

        public async Task Remove(List<LicenseUser> users)
        {
            using (var progressBar = new ProgressBar(users.Count, "Deleting users", ConsoleColor.White))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    progressBar.Tick($"Processing: {user.DisplayName} \t #{index}");

                    try
                    {
                        await _client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to delete: {user.DisplayName}");
                        Logger.Error($"Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }
        }

        public async Task Update(List<LicenseUser> users)
        {
            using (var progressBar = new ProgressBar(users.Count, "Updating users", ConsoleColor.White))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    progressBar.Tick($"Processing: {user.DisplayName} \t #{index}");

                    try
                    {
                        await _client.For<LicenseUser>().Key(user.Id).Set(new LicenseUser
                        {
                            DisplayName = user.DisplayName,
                            Email = user.Email,
                            Enabled = user.Enabled,
                            FirstName = user.FirstName,
                            Surname = user.Surname,
                            WhenCreated = user.WhenCreated
                        }).UpdateEntryAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to delete: {user.DisplayName}");
                        Logger.Error($"Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
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
                return await _client.For<SupportUpload>().Function("GetCallInStatus").ExecuteAsScalarAsync<CallInStatus>();
            }
            catch (Exception ex)
            {
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
                return await _client.Unbound<SupportUpload>().Function("GetUploadId").ExecuteAsScalarAsync<int>();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the upload id for device: {deviceId}");
                Logger.DebugFormat("Exception: ", ex);

                // default return from the api
                return 0;
            }
        }

        public async Task Update(SupportUpload upload)
        {
            try
            {
                upload = await _client.For<SupportUpload>().Key(upload.Id).Set(new SupportUpload
                {
                    CheckInTime = DateTime.Now,
                    Hostname = Environment.MachineName,
                    Status = CallInStatus.CalledIn                    
                }).UpdateEntryAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to update upload: {upload.Id}");
                Logger.DebugFormat("Exception: ", ex);
            }
        }

        public async Task Add(SupportUpload upload)
        {
            try
            {
                await _client.For<SupportUpload>().Set(upload).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to add upload");
                Logger.DebugFormat("Exception: ", ex);
            }
        }

        public async Task<SupportUpload> Get(int id)
        {
            try
            {
                var upload = await _client.For<SupportUpload>().Key(id).Expand(s => s.Users).FindEntryAsync();
                return upload;
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
                var users = await _client.For<SupportUpload>().Key(uploadId).NavigateTo(x => x.Users).FindEntryAsync();
                return users;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the users for upload: {uploadId}");
                Logger.DebugFormat("Exception: ", ex);
                return null;
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
                return await _client.Unbound().Function("GetAccountId").ExecuteAsScalarAsync<int>();

            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the account id for device: {deviceId}");
                Logger.DebugFormat("Exception: ", ex);

                throw;
            }
        }
    }
}