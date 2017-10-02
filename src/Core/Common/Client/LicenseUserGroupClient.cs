namespace Core.Common.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class LicenseUserGroupClient : LmsClientBase
    {
        protected PortalClient PortalClient { get; set; }
        /// <inheritdoc />
        public LicenseUserGroupClient()
            : base(new ODataPortalAuthenticationClientSettings())
        {
            PortalClient = new PortalClient();
        }

        public async Task Add(List<LicenseUser> users, LicenseGroup group)
        {
            foreach (LicenseUser user in users)
            {
                await Add(user, group);
            }
        }

        public async Task Add(LicenseUser user, LicenseGroup group)
        {
            try
            {
                await Client.For<LicenseUser>().Key(user.Id).LinkEntryAsync(group, "Groups");
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error adding user: {user.DisplayName} to group: {group.Name}");
                ex.Handle();
            }
        }

        public async Task Remove(List<LicenseUser> users, LicenseGroup group)
        {
            foreach (LicenseUser user in users)
            {
                await Remove(user, group);
            }
        }

        public async Task Remove(LicenseUser user, LicenseGroup group)
        {
            try
            {
                await PortalClient.RemoveUserFromGroup(user.Id, group.Id);
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error removing user: {user.DisplayName} from group: {group.Name}");
                ex.Handle();
            }
        }
    }
}