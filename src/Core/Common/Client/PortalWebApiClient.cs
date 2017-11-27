namespace LMS.Common.Client
{
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Abp.Threading;
    using Abp.WebApi.Client;
    using Core.Administration;
    using Core.Common.Constants;

    /// <summary>
    ///     Class used for standard api calls
    /// </summary>
    public class PortalWebApiClient : ITransientDependency
    {
        private readonly IAbpWebApiClient _abpWebApiClient;

        public PortalWebApiClient(IAbpWebApiClient abpWebApiClient)
        {
            BaseUrl = Constants.BaseServiceUrl;
            _abpWebApiClient = abpWebApiClient;
        }

        public string BaseUrl { get; set; }

        public async Task<string> GetAntiForgeryTokenAsync() => await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");

        public string GetAntiForgeryToken() => AsyncHelper.RunSync(GetAntiForgeryTokenAsync);
    }
}