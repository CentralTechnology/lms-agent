namespace LMS.Core.Services.Authentication
{
    using System;
    using Newtonsoft.Json;

    public class PortalToken
    {
        public string access_token { get; set; }

        public long expires_in {get;set;}

        public DateTime request_time { get; set; }
        public bool IsNearExpiry()
        {
            var now = DateTime.UtcNow;
            var expires = request_time.AddSeconds(expires_in).AddMinutes(-5);
            return now >= expires;
        }
    }
}