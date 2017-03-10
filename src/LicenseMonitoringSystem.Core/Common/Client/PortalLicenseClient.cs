namespace Core.Common.Client
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Abp;
    using Abp.Dependency;
    using Abp.Threading;
    using Abp.Web.Security.AntiForgery;
    using Castle.Core.Logging;
    using Microsoft.Data.OData;
    using Newtonsoft.Json;
    using Settings;
    using Simple.OData.Client;

    public class PortalLicenseClient : ODataClient, ITransientDependency
    {
        public PortalLicenseClient()
            : base(new DefaultLicenseClientSettings())
        {
            SettingManager = IocManager.Instance.Resolve<ISettingManager>();
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public ISettingManager SettingManager { get; set; }

        public void FormatWebRequestException(WebRequestException ex)
        {
            var response = JsonConvert.DeserializeObject<ODataResponseWrapper>(ex.Response);
            if (response != null)
            {
                Logger.Error($"Status: { response.Error.Code }");
                Logger.Error($"Message: {response.Error.Message}");

                if (response.Error.InnerError != null)
                {
                    Logger.Error($"Inner Message: {response.Error.InnerError.Message}");
                }
            }

            Logger.Debug($"Exception: {ex}");
        }
    }

    public class ODataResponseWrapper
    {
        public ODataResponse Error { get; set; }
    }
    public class ODataResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public InnerError InnerError { get; set; }
    }

    public class InnerError
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public string Stacktrace { get; set; }
    }

    public class DefaultLicenseClientSettings : ODataClientSettings, ITransientDependency
    {
        public DefaultLicenseClientSettings()
        {
            Logger = NullLogger.Instance;

            using (var settingManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
            {
                using (var portalClient = IocManager.Instance.ResolveAsDisposable<PortalClient>())
                {
                    // this.PayloadFormat = ODataPayloadFormat.Json;

                    BaseUri = new Uri(settingManager.Object.GetServiceUrl());
                    BeforeRequest += br =>
                    {
                        br.ShouldIncludeErrorDetail();
                        br.Headers.Clear();

                        //                  br.Headers.Add("Cache-Control", "no-cache");
                        //                  br.Headers.Add("Connection", "keep-alive");
                        //                  br.Headers.Add("Accept", "*/*");
                        //                  br.Headers.Add("Accept-Encoding", new[] {"gzip",
                        //"deflate",
                        //"sdch",
                        //"br"});
                        //                  br.Headers.Add("Accept-Language", new[] {"en-GB",
                        //"en; q=0.8"});
                        //                  br.Headers.Add("Authorization", "Device 61632137-e81e-9aad-5cc2-bc7de36c938f");
                        //                 // br.Headers.Add("Cookie", "Abp.Localization.CultureName=en; ASP.NET_SessionId=pb3bmuqqvtlqxgxoc1fygns0; __RequestVerificationToken=44_oxboRya955UXmmxMqG1RsdzZl4zKkikQnLpFLp9i-lCHfJIfodmtm5DNP66d5xeffijpfIFB_lOTpue_PyR7fslC0YbhM7Z5FBexc5X01; XSRF-TOKEN=kaSNq1MnQePdIsNTtHgUTSL74R9mDhJIZbZALE7hIRLZRvHVxwH7iScUY-vdu3xcz3HusrsP0KBO7Q_rfiNW6G8nagOl8sLPtXFeWdkCCuc1");
                        //                  br.Headers.Add("Host", "localhost:61814");
                        //                  br.Headers.Add("User-Agent", new[] {"Mozilla/5.0",
                        //"(Windows NT 10.0; WOW64)",
                        //"AppleWebKit/537.36",
                        //"(KHTML, like Gecko)",
                        //"Chrome/56.0.2924.87",
                        //"Safari/537.36"});
                        //                  br.Headers.Add("X-XSRF-TOKEN", "7Agnfzk1U7xe-0nJ3H0Jk_qJWELuJlS5GU8ifsnpP4kLUrcmTTCn6_xjapei--GcHbvqSECwadca5nMz4k3CMg4LPHuBgtv4_-QbZX1M_s41");
                        //                  br.Headers.Add("AccountId", "29683999");
                        //                  br.Headers.Add("DNT", "1");

                        //br.Headers.Add("cache-control", "no-cache");
                        ////br.Headers.Add("content-type", "application/json");
                        // ReSharper disable once AccessToDisposedClosure
                        br.Headers.Add("AccountId", settingManager.Object.GetAccountId().ToString());
                        // ReSharper disable once AccessToDisposedClosure
                        br.Headers.Add("XSRF-TOKEN", AsyncHelper.RunSync(() => portalClient.Object.GetTokenCookie()));
                        // ReSharper disable once AccessToDisposedClosure
                        br.Headers.Add("Authorization", $"Device {settingManager.Object.GetDeviceId()}");
                    };

                    //if (settingManager.Object.GetLogLevel(TODO))
                    //{
                    //    OnTrace += (x, y) => { Logger.Debug($"{x} {y}"); };
                    //}
                }
            }
        }

        public ILogger Logger { get; set; }
    }
}