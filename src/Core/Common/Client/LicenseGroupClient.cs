namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class LicenseGroupClient : DomainService, ILicenseGroupClient
    {
        public async Task Add(List<LicenseGroup> groups)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < groups.Count; index++)
            {
                LicenseGroup group = groups[index];

                try
                {
                    Logger.Debug($"Creating group: {group.Name}");

                    await client.For<LicenseGroup>().Set(new
                    {
                        group.Id,
                        group.Name,
                        group.WhenCreated
                    }).InsertEntryAsync();
                }
                catch (WebRequestException ex)
                {
                    Logger.Error($"Unable to create group: {group.Name}.");
                    ExceptionExtensions.HandleWebRequestException(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to create group: {group.Name}.");
                    Logger.Debug($"Group: {group.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task Remove(List<LicenseGroup> groups)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < groups.Count; index++)
            {
                LicenseGroup group = groups[index];

                try
                {
                    Logger.Debug($"Removing group: {group.Name}");

                    await client.For<LicenseGroup>().Key(group.Id).DeleteEntryAsync();
                }
                catch (WebRequestException ex)
                {
                    Logger.Error($"Unable to remove group: {group.Name}.");
                    ExceptionExtensions.HandleWebRequestException(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to remove group: {group.Name}.");
                    Logger.Debug($"Group: {group.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task Update(List<LicenseGroup> groups)
        {
            var client = new ODataClient(new ODataLicenseClientSettings());

            for (int index = 0; index < groups.Count; index++)
            {
                LicenseGroup group = groups[index];

                try
                {
                    Logger.Debug($"Updating group: {group.Name}");

                    await client.For<LicenseGroup>().Key(group.Id).Set(new
                    {
                        group.Name,
                        group.WhenCreated
                    }).UpdateEntryAsync();
                }
                catch (WebRequestException ex)
                {
                    Logger.Error($"Unable to update group: {group.Name}.");
                    ExceptionExtensions.HandleWebRequestException(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to update group: {group.Name}.");
                    Logger.Debug($"Group: {group.Dump()}");
                    Logger.Debug(ex.ToString());
                }
            }
        }

        public async Task<List<LicenseGroup>> GetAll()
        {
            try
            {
                var client = new ODataClient(new ODataLicenseClientSettings());
                IEnumerable<LicenseGroup> groups = await client.For<LicenseGroup>().FindEntriesAsync();
                return groups.ToList();
            }
            catch (WebRequestException ex)
            {
                ExceptionExtensions.HandleWebRequestException(ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of groups from the api.");
                Logger.Debug(ex.ToString());
                return null;
            }
        }
    }
}