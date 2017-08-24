namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using OData;

    public class ProfileClient : LmsClientBase
    {
        /// <inheritdoc />
        public ProfileClient()
            : base(new ODataProfileClientSettings())
        {
        }

        public async Task<int?> GetAccountByDeviceId(Guid deviceId)
        {
            return await Client.For("Profiles").Function("GetAccountId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
        }
    }
}