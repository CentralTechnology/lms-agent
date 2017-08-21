namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class LicenseUserClient : LmsClientBase
    {
        /// <inheritdoc />
        public LicenseUserClient()
            : base(new ODataPortalAuthenticationClientSettings())
        {
        }

        public async Task Add(List<LicenseUser> users)
        {
            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    await Client.For<LicenseUser>().Set(new
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
                catch (Exception ex)
                {
                    Logger.Error($"Unable to create user: {user.DisplayName}.");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task<List<LicenseUser>> GetAll()
        {
            try
            {
                var client = new ODataClient(new ODataPortalAuthenticationClientSettings());
                IEnumerable<LicenseUser> users = await client.For<LicenseUser>().Expand(u => u.Groups).FindEntriesAsync();
                List<LicenseUser> licenseUsers = users.ToList();
                Logger.Debug($"{licenseUsers.Count} users returned from the api.");
                return licenseUsers;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of users from the api.");
                Logger.Debug(ex);
                return null;
            }
        }

        public async Task Remove(List<LicenseUser> users)
        {
            var client = new ODataClient(new ODataPortalAuthenticationClientSettings());

            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    Logger.Debug($"Removing user: {user.DisplayName}");
                    await client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync();
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
            var client = new ODataClient(new ODataPortalAuthenticationClientSettings());

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
                catch (Exception ex)
                {
                    Logger.Error($"Unable to update user: {user.DisplayName}.");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }
    }
}