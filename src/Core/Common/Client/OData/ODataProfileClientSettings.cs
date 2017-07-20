﻿namespace Core.Common.Client.OData
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Constants;

    public class ODataProfileClientSettings : ODataCommonClientSettings
    {
        public ODataProfileClientSettings()
        {
            Validate();

            BaseUri = new Uri(Constants.DefaultServiceUrl);

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