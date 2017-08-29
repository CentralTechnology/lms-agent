namespace Core.Common.Client.OData
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Abp;
    using Administration;
    using Constants;
    using Extensions;
    using Helpers;

    public class ODataPortalAuthenticationClientSettings : ODataCommonClientSettings
    {
        public ODataPortalAuthenticationClientSettings()
        {
            Validate();

            BaseUri = new Uri(Constants.DefaultServiceUrl);

            BeforeRequest += br =>
            {
                br.ShouldIncludeErrorDetail();
                br.Headers.Clear();

                // ReSharper disable once AccessToDisposedClosure
                br.Headers.Add("AccountId", AccountId.ToString());
                // ReSharper disable once AccessToDisposedClosure
                br.Headers.Add("XSRF-TOKEN", Token);
                // ReSharper disable once AccessToDisposedClosure
                br.Headers.Authorization = new AuthenticationHeaderValue("Device", DeviceId.ToString("D").ToUpper());
            };
        }

        private static int AccountId { get; set; }

        private void Validate()
        {
            ValidateAccountId();

            ValidateDeviceId();

            ValidateToken();
        }

        private void ValidateAccountId()
        {
            if (AccountId == default(int))
            {
                AccountId = SettingManagerHelper.AccountId;
            }
        }
    }
}