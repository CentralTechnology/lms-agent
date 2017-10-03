namespace Core.Common.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class LicenseGroupClient : PortalODataClientBase
    {
        public async Task Add(IEnumerable<LicenseGroup> groups)
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
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseGroup>().Set(new
                {
                    group.Id,
                    group.Name,
                    group.WhenCreated
                }).InsertEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error creating group: {group.Id}");
                ex.Handle();
            }
        }

        public async Task<IEnumerable<LicenseGroup>> GetAll(ODataExpression<LicenseGroup> filter = null)
        {
            try
            {
                if (filter == null)
                {
                    return await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseGroup>().FindEntriesAsync());
                }
                    return await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseGroup>().Filter(filter).FindEntriesAsync());
            }
            catch (WebRequestException ex)
            {
                ex.Handle();
                return null;
            }
        }

        public async Task Remove(IEnumerable<LicenseGroup> groups)
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
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseGroup>().Key(group.Id).DeleteEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error removing group: {group.Id}");
                ex.Handle();
            }
        }

        public async Task Update(IEnumerable<LicenseGroup> groups)
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
                await DefaultPolicy.ExecuteAsync(() => Client.For<LicenseGroup>().Key(group.Id).Set(new
                {
                    group.Name,
                    group.WhenCreated
                }).UpdateEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error updating group: {group.Id}");
                ex.Handle();
            }
        }
    }
}