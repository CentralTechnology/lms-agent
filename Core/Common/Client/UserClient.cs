namespace LicenseMonitoringSystem.Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Actions;
    using Newtonsoft.Json;
    using Portal.Common.Enums;
    using Portal.License.User;
    using ShellProgressBar;
    using Tools;

    public class UserClient : PortalClientBase, IUserClient
    {
        public void Add(IList<LicenseUser> users)
        {
            using (var progressBar = new ProgressBar(users.Count, "Adding users", ConsoleColor.White))
            {
                for (int index = 0; index < users.Count; index++)
                {
                    var user = users[index];

                    progressBar.Tick($"Processing: {user.DisplayName} \t #{index}");

                    Container.AddToUsers(user);

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
                        Logger.DebugFormat("Created: {0}", JsonConvert.SerializeObject(user));
                        Container.Detach(user);
                    }
                }
            }
        }

        public void Remove(IList<LicenseUser> users)
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

        public void Update(IList<LicenseUser> users)
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

    public class UserUploadClient : PortalClientBase, IUserUploadClient
    {
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

        public LicenseUserUpload Get(int id)
        {
            // ReSharper disable once ReplaceWithSingleCallToSingleOrDefault
            return Container.UserUploads.Expand(u => u.Users).Where(u => u.Id.Equals(id)).SingleOrDefault();
        }

        public CallInStatus GetStatusByDeviceId(Guid deviceId)
        {
            AddAccountIdHeader();

            return Container.UserUploads.Status(deviceId).GetValue();
        }

        public int GetUploadIdByDeviceId(Guid deviceId)
        {
            return Container.UserUploads.Id(deviceId).GetValue();
        }

        public void Update(int id, LicenseUserUpload userUpload)
        {
            Container.UpdateObject(userUpload);
            var response = Container.SaveChanges();
            HandleResponse(response);
        }
    }

    public class ProfileClient : PortalClientBase, IProfileClient
    {
        public int GetAccountByDeviceId(Guid deviceId)
        {
            return Container.Profiles.AccountId(deviceId).GetValue();
        }
    }

    public class UserGroupClient : PortalClientBase, IUserGroupClient
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