namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Abp.Timing;
    using Extensions;
    using Models;
    using OData;
    using Simple.OData.Client;

    public class SupportUploadClient : LmsClientBase
    {
        /// <inheritdoc />
        public SupportUploadClient() : base(new ODataPortalAuthenticationClientSettings())
        {
        }

        public async Task<ManagedSupport> Add(ManagedSupport upload)
        {
            return await Client.For<ManagedSupport>().Set(upload).InsertEntryAsync();
        }

        public async Task<int> GetIdByDeviceId(Guid deviceId)
        {
            return await Client.For<ManagedSupport>().Function("GetUploadId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
        }

        public async Task<int> GetNewUploadId()
        {
            return await Client.For<ManagedSupport>().Function("NewUploadId").ExecuteAsScalarAsync<int>();
        }

        public async Task<CallInStatus> GetStatusByDeviceId(Guid deviceId)
        {
            try
            {
                return await Client.For<ManagedSupport>().Function("GetCallInStatus").Set(new {deviceId}).ExecuteAsScalarAsync<CallInStatus>();
            }
            catch (WebRequestException ex)
            {
                Logger.Error("Error getting the current upload status.");
                ex.Handle(Logger);

                return CallInStatus.NotCalledIn;
            }
        }

        public async Task Update(int id)
        {
            int uploadId = await GetNewUploadId();

            string version = SettingManager.GetClientVersion();

            try
            {
                await Client.For<ManagedSupport>().Key(id).Set(new
                {
                    CheckInTime = Clock.Now,
                    ClientVersion = version,
                    Hostname = Environment.MachineName,
                    Status = CallInStatus.CalledIn,
                    UploadId = uploadId
                }).UpdateEntryAsync();
            }
            catch (WebRequestException ex)
            {
                Logger.Error("Error calling in.");
                ex.Handle(Logger);
            }
        }
    }
}