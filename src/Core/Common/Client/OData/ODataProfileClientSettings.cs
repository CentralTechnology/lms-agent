namespace Core.Common.Client.OData
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Abp;
    using Abp.Dependency;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Simple.OData.Client;

    public class ODataProfileClientSettings : ODataClientSettings
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
        private static Guid DeviceId { get; set; }

        private static string Token { get; set; }

        private void Validate()
        {
            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                if (DeviceId == Guid.Empty)
                {
                    DeviceId = settingManager.Object.Read().DeviceId;

                    if (DeviceId == Guid.Empty)
                    {
                        throw new AbpException($"Cannot perform web request when device id is {Guid.Empty}");
                    }
                }

                if (Token.IsNullOrEmpty())
                {
                    using (IDisposableDependencyObjectWrapper<PortalClient> portalClient = IocManager.Instance.ResolveAsDisposable<PortalClient>())
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        Token = AsyncHelper.RunSync(() => portalClient.Object.GetTokenCookie());  
                    }
                }
            }
        }
    }
}