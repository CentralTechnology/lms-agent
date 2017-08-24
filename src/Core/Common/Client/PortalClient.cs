namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Abp;
    using Abp.Extensions;
    using Abp.Web.Models;
    using Abp.WebApi.Client;
    using Administration;
    using Constants;
    using Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    ///     Class used for standard api calls
    /// </summary>
    public class PortalClient
    {
        private readonly AbpWebApiClient _abpWebApiClient = new AbpWebApiClient();
        protected readonly SettingManager SettingManager = new SettingManager();
        public PortalClient()
        {
            BaseUrl = Constants.BaseServiceUrl;
        }

        public string BaseUrl { get; set; }

        public virtual async Task DeleteAsync(string url, int? timeout = null)
        {
            await DeleteAsync(url, null, timeout);
        }

        public virtual async Task DeleteAsync(string url, object input, int? timeout = null)
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
            {
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = timeout.HasValue ? TimeSpan.FromMilliseconds(timeout.Value) : _abpWebApiClient.Timeout;

                    if (!BaseUrl.IsNullOrEmpty())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    foreach (NameValue header in _abpWebApiClient.RequestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }

                    using (HttpResponseMessage response = await client.DeleteAsync(url))
                    {
                        SetResponseHeaders(response);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new AbpException("Could not made request to " + url + "! StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                        }
                    }
                }
            }
        }

        public async Task<string> GetTokenCookie()
        {
            return await _abpWebApiClient.PostAsync<string>($"{BaseUrl}/api/AntiForgery");
        }

        protected static TObj JsonString2Object<TObj>(string str)
        {
            return JsonConvert.DeserializeObject<TObj>(str,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        protected static string Object2JsonString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        public async Task RemoveUserFromGroup(Guid user, Guid group)
        {
            string url = $"{BaseUrl}/odata/v1/LicenseUsers({user})/Groups/$ref?$id={BaseUrl}/odata/v1/LicenseGroups({group})";

            int accountId = SettingManager.GetSettingValue<int>(SettingNames.AutotaskAccountId);
            var deviceId = SettingManager.GetSettingValue<Guid>(SettingNames.CentrastageDeviceId);

            _abpWebApiClient.RequestHeaders.Add(new NameValue("Authorization", $"Device {deviceId.ToString("D").ToUpper()}"));
            _abpWebApiClient.RequestHeaders.Add(new NameValue("AccountId", accountId.ToString()));

            await DeleteAsync(url);
        }

        protected void SetResponseHeaders(HttpResponseMessage response)
        {
            _abpWebApiClient.ResponseHeaders.Clear();
            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                foreach (string headerValue in header.Value)
                {
                    _abpWebApiClient.ResponseHeaders.Add(new NameValue(header.Key, headerValue));
                }
            }
        }
    }
}