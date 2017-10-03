namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using OData;
    using Simple.OData.Client;

    public class ProfileClient : PortalODataClientBase
    {
        public async Task<int?> GetAccountByDeviceId(Guid deviceId)
        {
            try
            {
                return await Client.For("Profiles").Function("GetAccountId").Set(new { deviceId }).ExecuteAsScalarAsync<int>();
            }
            catch (WebRequestException ex)
            {
                Logger.Error(ex.RequestUri);
                throw;
            }
            
        }
    }
}