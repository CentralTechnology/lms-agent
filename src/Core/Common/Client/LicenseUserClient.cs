namespace Core.Common.Client
{
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
            foreach (LicenseUser user in users)
            {
                await Add(user);
            }
        }

        public async Task Add(LicenseUser user)
        {
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
                    user.SamAccountName,
                    user.Surname,
                    user.WhenCreated
                }).InsertEntryAsync();
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    Logger.Error($"Error creating: {user.DisplayName} - {ex.Message}");
                    Logger.Debug(ex.Dump());
                }
                else
                {
                    Logger.Error($"Error creating: {user.DisplayName} - Http request timeout.");
                }
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error creating user: {user.DisplayName}");
                ex.Handle(Logger);
            }
        }

        public async Task<List<LicenseUser>> GetAll(ODataExpression<LicenseUser> filter = null)
        {
            try
            {
                IEnumerable<LicenseUser> users;
                if (filter == null)
                {
                    users = await Client.For<LicenseUser>()
                        .Expand(u => u.Groups)
                        .FindEntriesAsync();
                }
                else
                {
                    users = await Client.For<LicenseUser>()
                        .Expand(u => u.Groups)
                        .Filter(filter)
                        .FindEntriesAsync();
                }
                
                return users.ToList();
            }
            catch (WebRequestException ex)
            {
                ex.Handle(Logger);
                return null;
            }
        }

        public async Task Remove(List<LicenseUser> users)
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
                await Client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync();
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error removing user: {user.DisplayName}");
                ex.Handle(Logger);
            }
        }

        public async Task Update(List<LicenseUser> users)
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
                await Client.For<LicenseUser>().Key(user.Id).Set(new
                {
                    user.DisplayName,
                    user.Email,
                    user.Enabled,
                    user.FirstName,
                    user.LastLoginDate,
                    user.SamAccountName,
                    user.Surname,
                    user.WhenCreated
                }).UpdateEntryAsync();
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error updating user: {user.DisplayName}");
                ex.Handle(Logger);
            }
        }
    }
}