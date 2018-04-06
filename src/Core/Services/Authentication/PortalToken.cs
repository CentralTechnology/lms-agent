namespace LMS.Core.Services.Authentication
{
    using System;
    using Newtonsoft.Json;

    public class PortalToken
    {
        private long _expires_in;

        public string access_token { get; set; }

        public long expires_in
        {
            get => _expires_in;
            set
            {
                ComputeExpiry(value);
                _expires_in = value;
            }
        }

        [JsonIgnore]
        public DateTime ExpiresOn { get; set; }

        public DateTime request_time { get; set; }

        private void ComputeExpiry(long expiresIn)
        {
            ExpiresOn = DateTime.UtcNow.AddSeconds(expiresIn);
        }

        public bool IsNearExpiry()
        {
            return DateTime.UtcNow > ExpiresOn.AddMinutes(-1);
        }
    }
}