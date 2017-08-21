namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using OData;
    using Simple.OData.Client;

    public class ProfileClient
    {
        public async Task<int?> GetAccountByDeviceId(Guid deviceId)
        {
            var client = new ODataClient(new ODataProfileClientSettings());
            return await client.For("Profiles").Function("GetAccountId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
        }
    }
}