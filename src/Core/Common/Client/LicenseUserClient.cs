namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using Models;
    using NLog;
    using OData;
    using Simple.OData.Client;

    public class LicenseUserClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task Add(List<LicenseUser> users)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    Logger.Debug($"Creating user: {user.DisplayName}");

                    await client.For<LicenseUser>().Set(new
                    {
                        user.DisplayName,
                        user.Email,
                        user.Enabled,
                        user.FirstName,
                        user.Id,
                        user.LastLoginDate,
                        user.ManagedSupportId,
                        user.Surname,
                        user.WhenCreated
                    }).InsertEntryAsync();
                }
                catch (WebRequestException ex)
                {
                    Logger.Error($"Unable to create user: {user.DisplayName}.");
                    ExceptionExtensions.HandleWebRequestException(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to create user: {user.DisplayName}.");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task Remove(List<LicenseUser> users)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    Logger.Debug($"Removing user: {user.DisplayName}");
                    await client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync();
                }
                catch (WebRequestException ex)
                {
                    Logger.Error($"Unable to remove user: {user.DisplayName}.");
                    ExceptionExtensions.HandleWebRequestException(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to delete user: {user.DisplayName}");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task Update(List<LicenseUser> users)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    Logger.Debug($"Updating user: {user.DisplayName}");

                    await client.For<LicenseUser>().Key(user.Id).Set(new
                    {
                        user.DisplayName,
                        user.Email,
                        user.Enabled,
                        user.FirstName,
                        user.LastLoginDate,
                        user.Surname,
                        user.WhenCreated
                    }).UpdateEntryAsync();
                }
                catch (WebRequestException ex)
                {
                    Logger.Error($"Unable to update user: {user.DisplayName}.");
                    ExceptionExtensions.HandleWebRequestException(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to update user: {user.DisplayName}.");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task<List<LicenseUser>> GetAll()
        {
            try
            {
                var client = new ODataClient(new ODataLicenseClientSettings());
                IEnumerable<LicenseUser> users = await client.For<LicenseUser>().Expand(u => u.Groups).FindEntriesAsync();
                List<LicenseUser> licenseUsers = users.ToList();
                Logger.Debug($"{licenseUsers.Count} users returned from the api.");
                return licenseUsers;
            }
            catch (WebRequestException ex)
            {
                ExceptionExtensions.HandleWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of users from the api.");
                Logger.Debug(ex);
                return null;
            }
        }
    }
}