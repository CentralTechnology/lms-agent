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
        private readonly AbpWebApiClient _abpWebApiClient;
        protected readonly SettingManager SettingManager = new SettingManager();

        public PortalWebApiClient()
            :this(new AbpWebApiClient())
        {
            
        }

        public PortalWebApiClient(AbpWebApiClient abpWebApiClient)
        {
            BaseUrl = Constants.BaseServiceUrl;
            _abpWebApiClient = abpWebApiClient;
        }

        public string BaseUrl { get; set; }

        public virtual async Task<string> GetTokenCookie()
        {
            return await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");
        }
    }
}