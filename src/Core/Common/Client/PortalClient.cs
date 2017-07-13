namespace Core.Common.Client
{
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Abp.WebApi.Client;

    /// <summary>
    ///     Class used for standard api calls
    /// </summary>
    public class PortalClient
    {
        private readonly AbpWebApiClient _abpWebApiClient = new AbpWebApiClient();

        public PortalClient()
        {
            BaseUrl = LmsConstants.BaseServiceUrl;;
        }

        public string BaseUrl { get; set; }

        public async Task<string> GetTokenCookie()
        {
            return await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");
        }
    }
}