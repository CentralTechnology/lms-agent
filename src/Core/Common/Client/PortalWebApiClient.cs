namespace Core.Common.Client
{
    using System.Threading.Tasks;
    using Abp.WebApi.Client;
    using Administration;
    using Constants;

    /// <summary>
    ///     Class used for standard api calls
    /// </summary>
    public class PortalWebApiClient
    {
        private readonly AbpWebApiClient _abpWebApiClient = new AbpWebApiClient();
        protected readonly SettingManager SettingManager = new SettingManager();

        public PortalWebApiClient()
        {
            BaseUrl = Constants.BaseServiceUrl;
        }

        public string BaseUrl { get; set; }

        public async Task<string> GetTokenCookie()
        {
            return await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");
        }
    }
}