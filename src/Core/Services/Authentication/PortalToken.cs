namespace LMS.Core.Services.Authentication
{
    using System;
    using Newtonsoft.Json;

    public class PortalToken
    {
        private PortalToken()
        {
            ExpiresOn = DateTime.UtcNow.AddSeconds(expires_in);
        }

        public string access_token { get; set; }
        public long expires_in { get; set; }

        [JsonIgnore]
        public DateTime ExpiresOn { get; set; }

        public DateTime request_time { get; set; }

        public bool IsNearExpiry()
        {
            return DateTime.UtcNow > ExpiresOn.AddMinutes(-1);
        }
    }
}