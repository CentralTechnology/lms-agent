namespace LMS.Core.Services.Authentication
{
    using System;
    using Newtonsoft.Json;

    public class PortalToken
    {
        private PortalToken(){}

        public static PortalToken Create(string accessToken, long expiresIn, DateTime requestTime)
        {
            return new PortalToken
            {
                AccessToken = accessToken,
                ExpiresIn = expiresIn,
                RequestTime = requestTime
            };
        }

        public void Update(string accessToken, long expiresIn, DateTime requestTime)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RequestTime = requestTime;
        }


        public string AccessToken { get; set; }

        public long ExpiresIn {get;set;}

        public DateTime RequestTime { get; set; }

        public bool IsNearExpiry
        {
            get
            {
                var now = DateTime.Now;
                var expires = RequestTime.AddSeconds(ExpiresIn).AddMinutes(-5);
                return now >= expires;
            }
        }
    }
}