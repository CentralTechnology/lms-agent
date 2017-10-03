namespace Core.Common.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class LicenseUserGroupClient : PortalODataClientBase
    {
        public async Task Add(IEnumerable<LicenseUser> users, LicenseGroup licenseGroup)
        {
            foreach (LicenseUser user in users)
            {
                await Add(user, licenseGroup);
            }
        }

        public async Task Add(LicenseUser user, LicenseGroup licenseGroup)
        {
            try
            {
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseUser>().Key(user.Id).LinkEntryAsync(licenseGroup, "Groups"));
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error adding user: {user.DisplayName} to group: {licenseGroup.Name}");
                ex.Handle();
            }
        }

        public async Task Remove(IEnumerable<LicenseUser> users, LicenseGroup licenseGroup)
        {
            foreach (LicenseUser user in users)
            {
                await Remove(user, licenseGroup);
            }
        }

        public async Task Remove(LicenseUser user, LicenseGroup licenseGroup)
        {
            try
            {
                await DefaultPolicy.ExecuteAsync(() => new PortalClient().RemoveUserFromGroup(user.Id, licenseGroup.Id));
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error removing user: {user.DisplayName} from group: {licenseGroup.Name}");
                ex.Handle();
            }
        }
    }
}