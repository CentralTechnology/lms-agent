namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using NLog;
    using OData;
    using Simple.OData.Client;

    public class ProfileClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<int> GetAccountByDeviceId(Guid deviceId)
        {
            try
            {
                var client = new ODataClient(new ODataProfileClientSettings());
                return await client.For("Profiles").Function("GetAccountId").Set(new {deviceId}).ExecuteAsScalarAsync<int>();
            }
            catch (WebRequestException ex)
            {
                ExceptionExtensions.HandleWebRequestException(ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get the account id for device: {deviceId}");
                Logger.Debug(ex);

                throw;
            }
        }
    }
}