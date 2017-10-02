namespace Core.Common.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class LicenseGroupClient : LmsClientBase
    {
        /// <inheritdoc />
        public LicenseGroupClient()
            : base(new ODataPortalAuthenticationClientSettings())
        {
        }

        public async Task Add(List<LicenseGroup> groups)
        {
            foreach (LicenseGroup group in groups)
            {
                await Add(group);
            }
        }

        public async Task Add(LicenseGroup group)
        {
            try
            {
                await Client.For<LicenseGroup>().Set(new
                {
                    group.Id,
                    group.Name,
                    group.WhenCreated
                }).InsertEntryAsync();
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error creating group: {group.Id}");
                ex.Handle();
            }
        }

        public async Task<List<LicenseGroup>> GetAll(ODataExpression<LicenseGroup> filter = null)
        {
            try
            {
                IEnumerable<LicenseGroup> groups;
                if (filter == null)
                {
                    groups = await Client.For<LicenseGroup>().FindEntriesAsync();
                }
                else
                {
                    groups = await Client.For<LicenseGroup>().Filter(lg => !lg.IsDeleted).FindEntriesAsync();
                }
                 
                return groups.ToList();
            }
            catch (WebRequestException ex)
            {
                ex.Handle();
                return null;
            }
        }

        public async Task Remove(List<LicenseGroup> groups)
        {
            foreach (LicenseGroup group in groups)
            {
                await Remove(group);
            }
        }

        public async Task Remove(LicenseGroup group)
        {
            try
            {
                await Client.For<LicenseGroup>().Key(group.Id).DeleteEntryAsync();
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error removing group: {group.Id}");
                ex.Handle();
            }
        }

        public async Task Update(List<LicenseGroup> groups)
        {
            foreach (LicenseGroup group in groups)
            {
                await Update(group);
            }
        }

        public async Task Update(LicenseGroup group)
        {
            try
            {
                await Client.For<LicenseGroup>().Key(group.Id).Set(new
                {
                    group.Name,
                    group.WhenCreated
                }).UpdateEntryAsync();
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error updating group: {group.Id}");
                ex.Handle();
            }
        }
    }
}