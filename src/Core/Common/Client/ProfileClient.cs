namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using OData;
    using Simple.OData.Client;

    public class ProfileClient : DomainService, IProfileClient
    {
        public async Task<int> GetAccountByDeviceId(Guid deviceId)
        {
            try
            {
                var client = new ODataClient(new ODataProfileClientSettings());
                return await client.For("Profiles").Function("GetAccountId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
            }
            catch (WebRequestException ex)
            {
                ex.FormatWebRequestException();
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the account id for device: {deviceId}");
                Logger.DebugFormat("Exception: ", ex);

                throw;
            }
        }
    }
}