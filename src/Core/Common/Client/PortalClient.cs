using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Common.Client
{
    using Abp;
    using Abp.Dependency;
    using Abp.WebApi.Client;
    using Settings;

    /// <summary>
    ///  Class used for standard api calls
    /// </summary>
    public class PortalClient : ITransientDependency
    {
        public string BaseUrl { get; set; }

        public PortalClient(IAbpWebApiClient abpWebApiClient)
        {
            BaseUrl = Setting.BaseServiceUrl;
            _abpWebApiClient = abpWebApiClient;
        }

        private readonly IAbpWebApiClient _abpWebApiClient;

        public async Task<string> GetTokenCookie()
        {
            return await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");
        }
    }
}
