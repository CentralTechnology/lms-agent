namespace Core.Common.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using Simple.OData.Client;

    public class LicenseUserClient : PortalODataClientBase
    {
        public async Task Add(List<LicenseUser> users)
        {
            foreach (LicenseUser user in users)
            {
                await Add(user);
            }
        }

        public async Task Add(LicenseUser user)
        {
            try
            {
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseUser>().Set(new
                {
                    user.DisplayName,
                    user.Email,
                    user.Enabled,
                    user.FirstName,
                    user.Id,
                    user.LastLoginDate,
                    user.ManagedSupportId,
                    user.SamAccountName,
                    user.Surname,
                    user.WhenCreated
                }).InsertEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error creating user: {user.DisplayName}");
                ex.Handle();
            }
        }

        public async Task<IList<LicenseUser>> GetAll(ODataExpression<LicenseUser> filter = null)
        {
            try
            {
                if (filter == null)
                {
                    return new List<LicenseUser>(await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseUser>()
                        .Expand(u => u.Groups)                        
                        .FindEntriesAsync()));
                }

                return new List<LicenseUser>(await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseUser>()
                    .Expand(u => u.Groups)
                    .Filter(filter)                                     
                    .FindEntriesAsync()));
            }
            catch (WebRequestException ex)
            {
                ex.Handle();
                throw;
            }
        }

        public async Task Remove(IEnumerable<LicenseUser> users)
        {
            foreach (LicenseUser user in users)
            {
                await Remove(user);
            }
        }

        public async Task Remove(LicenseUser user)
        {
            try
            {
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error removing user: {user.DisplayName}");
                ex.Handle();
            }
        }

        public async Task Update(IEnumerable<LicenseUser> users)
        {
            foreach (LicenseUser user in users)
            {
                await Update(user);
            }
        }

        public async Task Update(LicenseUser user)
        {
            try
            {
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseUser>().Key(user.Id).Set(new
                {
                    user.DisplayName,
                    user.Email,
                    user.Enabled,
                    user.FirstName,
                    user.LastLoginDate,
                    user.SamAccountName,
                    user.Surname,
                    user.WhenCreated
                }).UpdateEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error updating user: {user.DisplayName}");
                ex.Handle();
            }
        }
    }
}