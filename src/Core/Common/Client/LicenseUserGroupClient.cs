namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using NLog;
    using OData;
    using Simple.OData.Client;

    public class LicenseUserGroupClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task Add(List<LicenseUser> users, LicenseGroup group)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    Logger.Debug($"Adding user: {user.DisplayName} to group: {group}");

                    await client.For<LicenseUser>().Key(user.Id).LinkEntryAsync(group, "Groups");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to add {user.DisplayName} to {group.Name}");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task Remove(List<LicenseUser> users, LicenseGroup group)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < users.Count; index++)
            {
                LicenseUser user = users[index];

                try
                {
                    Logger.Debug($"Removing user: {user.DisplayName} from group: {group}");

                    await client.For<LicenseUser>().Key(user.Id).UnlinkEntryAsync(group, "Groups");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to remove {user.DisplayName} from {group.Name}");
                    Logger.Debug($"User: {user.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }
    }
}