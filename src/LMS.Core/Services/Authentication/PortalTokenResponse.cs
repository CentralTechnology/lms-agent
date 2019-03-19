namespace LMS.Core.Services.Authentication
{
    using System;
    using Newtonsoft.Json;

    public class PortalTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("request_time")]
        public DateTime RequestTime { get; set; }
    }
}