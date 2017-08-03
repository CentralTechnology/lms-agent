namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using NLog;
    using OData;
    using Simple.OData.Client;
    using Veeam;

    public class VeeamClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ODataClient _client;

        public VeeamClient()
        {
            _client = new ODataClient(new ODataPortalAuthenticationClientSettings());
        }

        public async Task Add(Veeam veeam)
        {
            try
            {
                await _client.For<Veeam>()
                    .Set(new
                    {
                        veeam.CheckInTime,
                        veeam.ClientVersion,
                        veeam.Edition,
                        veeam.ExpirationDate,
                        veeam.HyperV,
                        veeam.Id,
                        veeam.LicenseType,
                        veeam.ProgramVersion,
                        veeam.Status,
                        veeam.SupportId,
                        veeam.TenantId,
                        veeam.UploadId,
                        veeam.vSphere
                    }).InsertEntryAsync().ConfigureAwait(false);
            }
            catch (WebRequestException ex)
            {
                ex.Handle(Logger);
                throw;
            }
        }

        public async Task<CallInStatus> GetStatus(Guid key)
        {
            try
            {
                return await _client.For<Veeam>().Function("GetCallInStatus").Set(new {key}).ExecuteAsScalarAsync<CallInStatus>();
            }
            catch (WebRequestException ex)
            {
                ex.Handle(Logger);
                throw;
            }
        }

        public async Task Update(Veeam veeam)
        {
            try
            {
                await _client.For<Veeam>()
                    .Key(veeam.Id)
                    .Set(new
                    {
                        veeam.CheckInTime,
                        veeam.ClientVersion,
                        veeam.Edition,
                        veeam.ExpirationDate,
                        veeam.HyperV,
                        veeam.LicenseType,
                        veeam.ProgramVersion,
                        veeam.SupportId,
                        veeam.TenantId,
                        veeam.Status,
                        veeam.UploadId,
                        veeam.vSphere
                    }).UpdateEntryAsync().ConfigureAwait(false);
            }
            catch (WebRequestException ex)
            {
                ex.Handle(Logger);
                throw;
            }
        }

        public async Task<int> UploadId()
        {
            try
            {
                return await _client.For<Veeam>().Function("NewUploadId").ExecuteAsScalarAsync<int>().ConfigureAwait(false);
            }
            catch (WebRequestException ex)
            {
                ex.Handle(Logger);
                throw;
            }
        }
    }
}