using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Client
{
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

        public async Task<CallInStatus> GetStatus(Guid key)
        {
            return await _client.For<Veeam>().Function("GetCallInStatus").Set(new {key}).ExecuteAsScalarAsync<CallInStatus>();
        }
    }
}
