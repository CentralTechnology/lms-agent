namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Models;
    using OData;
    using Simple.OData.Client;
    using Veeam;

    public class VeeamClient
    {
        private readonly ODataClient _client;

        public VeeamClient()
        {
            _client = new ODataClient(new ODataPortalAuthenticationClientSettings());
        }

        public async Task Add(Veeam veeam)
        {
            await _client.For<Veeam>()
                .Set(new
                {
                    veeam.ClientVersion,
                    veeam.Edition,
                    veeam.ExpirationDate,
                    veeam.HyperV,
                    veeam.Id,
                    veeam.LicenseType,
                    veeam.ProgramVersion,
                    veeam.SupportId,
                    veeam.TenantId,
                    veeam.vSphere
                }).InsertEntryAsync();
        }

        public async Task<CallInStatus> GetStatus(Guid key)
        {
            return await _client.For<Veeam>().Function("GetCallInStatus").Set(new {key}).ExecuteAsScalarAsync<CallInStatus>();
        }

        public async Task Update(Veeam veeam)
        {
            await _client.For<Veeam>()
                .Key(veeam.Id)
                .Set(new
                {
                    veeam.ClientVersion,
                    veeam.Edition,
                    veeam.ExpirationDate,
                    veeam.HyperV,
                    veeam.LicenseType,
                    veeam.ProgramVersion,
                    veeam.SupportId,
                    veeam.TenantId,
                    veeam.vSphere
                }).UpdateEntryAsync();
        }
    }
}