namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Abp.Timing;
    using Extensions;
    using Helpers;
    using Models;
    using ServiceStack;
    using ServiceStack.Text;
    using Simple.OData.Client;

    public class SupportUploadClient : PortalODataClientBase
    {
        public async Task<ManagedSupport> Add(ManagedSupport upload)
        {
            try
            {
                return await DefaultPolicy.ExecuteAsync(() => Client.For<ManagedSupport>().Set(upload).InsertEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error("Error getting the current upload status.");
                ex.Handle();

                throw;
            }
        }

        public async Task<int> GetIdByDeviceId(Guid deviceId)
        {
            return await DefaultPolicy.ExecuteAsync(() => Client.For<ManagedSupport>().Function("GetUploadId").Set(new {deviceId}).ExecuteAsScalarAsync<int>());
        }

        public async Task<int> GetNewUploadId()
        {
            return await DefaultPolicy.ExecuteAsync(() => Client.For<ManagedSupport>().Function("NewUploadId").ExecuteAsScalarAsync<int>());
        }

        public async Task<CallInStatus> GetStatusByDeviceId(Guid deviceId)
        {
            try
            {
                return await DefaultPolicy.ExecuteAsync(() => Client.For<ManagedSupport>().Function("GetCallInStatus").Set(new {deviceId}).ExecuteAsScalarAsync<CallInStatus>());
            }
            catch (WebRequestException ex)
            {
                Logger.Error("Error getting the current upload status.");
                ex.Handle();

                return CallInStatus.NotCalledIn;
            }
        }

        public async Task Update(int id)
        {
            int uploadId = await GetNewUploadId();

            string version = SettingManagerHelper.ClientVersion;

            try
            {
                await DefaultPolicy.ExecuteAsync(() => Client.For<ManagedSupport>()
                .Key(id)
                .Set(new
                    {
                        CheckInTime = new DateTimeOffset(Clock.Now),
                        ClientVersion = version,
                        Hostname = Environment.MachineName,
                        Status = CallInStatus.CalledIn.ToString(),
                        UploadId = uploadId.ToString()
                    })
                    .UpdateEntryAsync());
            }
            catch (WebRequestException ex)
            {
                Logger.Error($"{ex.Dump()}");
                throw;
            }
                
        }
    }
}