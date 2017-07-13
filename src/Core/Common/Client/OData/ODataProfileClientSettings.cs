namespace Core.Common.Client.OData
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class ODataProfileClientSettings : ODataCommonClientSettings
    {
        public ODataProfileClientSettings()
        {
            Validate();

            BaseUri = new Uri(LmsConstants.DefaultServiceUrl);

            BeforeRequest += br =>
            {
                br.ShouldIncludeErrorDetail();
                br.Headers.Clear();

                br.Headers.Add("XSRF-TOKEN", Token);
                br.Headers.Authorization = new AuthenticationHeaderValue("Device", DeviceId.ToString("D").ToUpper());
            };
        }

        private void Validate()
        {
            ValidateDeviceId();

            ValidateToken();
        }
    }
}