namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using ServiceStack.Text;
    using Simple.OData.Client;
    using Veeam;

    public class VeeamClient : PortalODataClientBase
    {
        public async Task Add(Veeam veeam)
        {
            await DefaultPolicy.ExecuteAsync(() => Client.For<Veeam>()
                .Set(new
                {
                    veeam.CheckInTime,
                    veeam.ClientVersion,
                    Edition = ((int) veeam.Edition).ToString(),
                    veeam.ExpirationDate,
                    veeam.HyperV,
                    veeam.Id,
                    LicenseType = ((int) veeam.LicenseType).ToString(),
                    veeam.ProgramVersion,
                    Status = ((int) veeam.Status).ToString(),
                    veeam.SupportId,
                    veeam.TenantId,
                    veeam.UploadId,
                    veeam.vSphere
                }).InsertEntryAsync());
        }

        public async Task<CallInStatus> GetStatus(Guid key)
        {
            try
            {
                return await DefaultPolicy.ExecuteAsync(() => Client.For<Veeam>().Function("GetCallInStatus").Set(new {key}).ExecuteAsScalarAsync<CallInStatus>());
            }
            catch (WebRequestException ex)
            {
                Logger.Error("Error getting the current upload status.");
                ex.Handle();

                return CallInStatus.NotCalledIn;
            }
        }

        public async Task Update(Veeam veeam)
        {
            try
            {
                await DefaultPolicy.ExecuteAsync(() => Client.For<Veeam>()
                    .Key(veeam.Id)
                    .Set(new
                    {
                        veeam.CheckInTime,
                        veeam.ClientVersion,
                        Edition = ((int) veeam.Edition).ToString(),
                        veeam.ExpirationDate,
                        veeam.HyperV,
                        LicenseType = ((int) veeam.LicenseType).ToString(),
                        veeam.ProgramVersion,
                        veeam.SupportId,
                        veeam.TenantId,
                        Status = ((int) veeam.Status).ToString(),
                        veeam.UploadId,
                        veeam.vSphere
                    }).UpdateEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"Error updating: {veeam.Dump()}");
                ex.Handle();
                throw;
            }
        }

        public async Task<int> UploadId()
        {
            return await DefaultPolicy.ExecuteAsync(() => Client.For<Veeam>().Function("NewUploadId").ExecuteAsScalarAsync<int>());
        }
    }
}