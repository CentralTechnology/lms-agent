namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;
    using ShellProgressBar;

    public class UserLicenseClient : LicenseMonitoringBase, IUserClient
    {
        private readonly PortalLicenseClient _client;

        public UserLicenseClient(PortalLicenseClient client)
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

        public Task Remove(List<LicenseUser> users)
        {
            using (var progressBar = new ProgressBar(users.Count, "Deleting users", ConsoleColor.White))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    progressBar.Tick($"Processing: {user.DisplayName} \t #{index}");

                    Container.DeleteObject(user);

                    try
                    {
                        var response = Container.SaveChanges();
                        HandleResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(ex.Message);
                        Logger.DebugFormat("Exception: ", ex);
                    }
                    finally
                    {
                        Logger.DebugFormat("Deleted: {0}", JsonConvert.SerializeObject(user));
                        Container.Detach(user);
                    }
                }
            }
        }

        public Task Update(List<LicenseUser> users)
        {
            using (var progressBar = new ProgressBar(users.Count, "Updating users", ConsoleColor.White))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    progressBar.Tick($"Processing: {user.DisplayName} \t #{index}");

                    Container.AttachTo("Users", user);
                    Container.UpdateObject(user);

                    try
                    {
                        var response = Container.SaveChanges();
                        HandleResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(ex.Message);
                        Logger.DebugFormat("Exception: ", ex);
                    }
                    finally
                    {
                        Logger.DebugFormat("Updated: {0}", JsonConvert.SerializeObject(user));
                        Container.Detach(user);
                    }
                }
            }
        }
    }

    public class SupportUploadLicenseClient : PortalLicenseClient, ISupportUploadClient
    {
        private readonly PortalLicenseClient _client;

        public SupportUploadLicenseClient(PortalLicenseClient client)
        {
            _client = client;
        }

        public void Add(LicenseUserUpload entity)
        {
            Container.AddToUserUploads(entity);

            try
            {
                var serviceResponse = Container.SaveChanges();
                HandleResponse(serviceResponse);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex.Message);
                Logger.DebugFormat("Exception: ", ex);
            }
        }

        public CallInStatus GetStatusByDeviceId(Guid deviceId)
        {
            AddAccountIdHeader();

            return Container.UserUploads.Status(deviceId).GetValue();
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
    }

    public class ProfileLicenseClient : PortalLicenseClient, IProfileClient
    {
        public int GetAccountByDeviceId(Guid deviceId)
        {
            return Container.Profiles.AccountId(deviceId).GetValue();
        }
    }

    public class UserGroupLicenseClient : PortalLicenseClient, IUserGroupClient
    {
        public void Add(IList<LicenseUserGroup> groups)
        {
            using (var progressBar = new ProgressBar(groups.Count, "Adding user groups", ConsoleColor.White))
            {
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];

                    progressBar.Tick($"Processing: {group.Name} \t #{index}");

                    Container.AddToUserGroups(group);

                    try
                    {
                        var response = Container.SaveChanges();
                        HandleResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(ex.Message);
                        Logger.DebugFormat("Exception: ", ex);
                    }
                    finally
                    {
                        Logger.DebugFormat("Created: {0}", JsonConvert.SerializeObject(group));
                        Container.Detach(group);
                    }
                }
            }
        }

        public void Remove(IList<LicenseUserGroup> groups)
        {
            using (var progressBar = new ProgressBar(groups.Count, "Deleting user groups", ConsoleColor.White))
            {
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];

                    progressBar.Tick($"Processing: {group.Name} \t #{index}");

                    Container.DeleteObject(group);

                    try
                    {
                        var response = Container.SaveChanges();
                        HandleResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(ex.Message);
                        Logger.DebugFormat("Exception: ", ex);
                    }
                    finally
                    {
                        Logger.DebugFormat("Deleted: {0}", JsonConvert.SerializeObject(group));
                        Container.Detach(group);
                    }
                }
            }
        }

        public void Update(IList<LicenseUserGroup> groups)
        {
            using (var progressBar = new ProgressBar(groups.Count, "Updating user groups", ConsoleColor.White))
            {
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];

                    progressBar.Tick($"Processing: {group.Name} \t #{index}");

                    Container.AttachTo("UserGroups", group);
                    Container.UpdateObject(group);

                    try
                    {
                        var response = Container.SaveChanges();
                        HandleResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(ex.Message);
                        Logger.DebugFormat("Exception: ", ex);
                    }
                    finally
                    {
                        Logger.DebugFormat("Updated: {0}", JsonConvert.SerializeObject(group));
                        Container.Detach(group);
                    }
                }
            }
        }
    }
}