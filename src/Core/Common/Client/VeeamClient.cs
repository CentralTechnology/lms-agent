namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;
    using Veeam;

    public class VeeamClient : LmsClientBase
    {
        /// <inheritdoc />
        public VeeamClient()
            : base(new ODataPortalAuthenticationClientSettings())
        {
        }

        public async Task Add(Veeam veeam)
        {
            await Client.For<Veeam>()
                .Set(new
                {
                    veeam.CheckInTime,
                    veeam.ClientVersion,
                    Edition = ((int)veeam.Edition).ToString(),
                    veeam.ExpirationDate,
                    veeam.HyperV,
                    veeam.Id,
                    LicenseType = ((int)veeam.LicenseType).ToString(),
                    veeam.ProgramVersion,
                    Status = ((int)veeam.Status).ToString(),
                    veeam.SupportId,
                    veeam.TenantId,
                    veeam.UploadId,
                    veeam.vSphere
                }).InsertEntryAsync();
        }

        public async Task<CallInStatus> GetStatus(Guid key)
        {
            try
            {
                return await Client.For<Veeam>().Function("GetCallInStatus").Set(new { key }).ExecuteAsScalarAsync<CallInStatus>();
            }
            catch (WebRequestException ex)
            {
                Logger.Error("Error getting the current upload status.");
                ex.Handle(Logger);

                return CallInStatus.NotCalledIn;
            }
        }

        public async Task Update(Veeam veeam)
        {
            await Client.For<Veeam>()
                .Key(veeam.Id)
                .Set(new
                {
                    veeam.CheckInTime,
                    veeam.ClientVersion,
                    Edition = ((int)veeam.Edition).ToString(),
                    veeam.ExpirationDate,
                    veeam.HyperV,
                    LicenseType = ((int)veeam.LicenseType).ToString(),
                    veeam.ProgramVersion,
                    veeam.SupportId,
                    veeam.TenantId,
                    Status = ((int)veeam.Status).ToString(),
                    veeam.UploadId,
                    veeam.vSphere
                }).UpdateEntryAsync();
        }

        public async Task<int> UploadId()
        {
            return await Client.For<Veeam>().Function("NewUploadId").ExecuteAsScalarAsync<int>();
        }
    }
}