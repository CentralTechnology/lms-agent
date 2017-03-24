namespace Core.Common.Client
{
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Abp.WebApi.Client;

    /// <summary>
    ///     Class used for standard api calls
    /// </summary>
    public class PortalClient : ITransientDependency
    {
        private readonly IAbpWebApiClient _abpWebApiClient;

        public PortalClient(IAbpWebApiClient abpWebApiClient)
        {
            BaseUrl = LmsConstants.BaseServiceUrl;
            _abpWebApiClient = abpWebApiClient;
        }

        public string BaseUrl { get; set; }

        public async Task<string> GetTokenCookie()
        {
            return await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");
        }
    }
}