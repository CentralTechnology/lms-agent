namespace Core.Common.Client.OData
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Abp;
    using Abp.Dependency;
    using Administration;

    public class ODataLicenseClientSettings : ODataCommonClientSettings
    {
        public ODataLicenseClientSettings()
        {
            Validate();

            BaseUri = new Uri(LmsConstants.DefaultServiceUrl);

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
            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                if (AccountId == 0)
                {
                    AccountId = settingManager.Object.Read().AccountId;

                    if (AccountId == 0)
                    {
                        throw new AbpException("Cannot perform web request when account id is 0");
                    }
                }
            }
        }
    }
}