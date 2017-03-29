namespace Core.Common.Client
{
    using System;
    using System.Net.Http;
    using Abp.Dependency;
    using Abp.Threading;
    using Administration;
    using Castle.Core.Logging;
    using Newtonsoft.Json;
    using Simple.OData.Client;

    public class PortalLicenseClient : ODataClient, ITransientDependency
    {
        public PortalLicenseClient()
            : base(new DefaultLicenseClientSettings())
        {
            SettingManager = IocManager.Instance.Resolve<ISettingsManager>();
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public ISettingsManager SettingManager { get; set; }

        public virtual void FormatWebRequestException(WebRequestException ex)
        {
            ODataResponseWrapper response = JsonConvert.DeserializeObject<ODataResponseWrapper>(ex.Response);
            if (response != null)
            {
                Logger.Error($"Status: {response.Error.Code}");
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
        public InnerError InnerError { get; set; }
        public string Message { get; set; }
    }

    public class InnerError
    {
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        public string Type { get; set; }
    }

    public class DefaultLicenseClientSettings : ODataClientSettings, ITransientDependency
    {
        public DefaultLicenseClientSettings()
        {
            Logger = NullLogger.Instance;

            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                using (IDisposableDependencyObjectWrapper<PortalClient> portalClient = IocManager.Instance.ResolveAsDisposable<PortalClient>())
                {
                    BaseUri = new Uri(LmsConstants.DefaultServiceUrl);
                    var deviceId = settingManager.Object.Read().DeviceId;
                    BeforeRequest += br =>
                    {
                        br.ShouldIncludeErrorDetail();
                        br.Headers.Clear();

                        // ReSharper disable once AccessToDisposedClosure
                        br.Headers.Add("AccountId", settingManager.Object.Read().AccountId.ToString());
                        // ReSharper disable once AccessToDisposedClosure
                        br.Headers.Add("XSRF-TOKEN", AsyncHelper.RunSync(() => portalClient.Object.GetTokenCookie()));
                        // ReSharper disable once AccessToDisposedClosure
                        br.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Device", deviceId.ToString());
                    };

                    if (settingManager.Object.ReadLoggerLevel() == LoggerLevel.Debug)
                    {
                        OnTrace += (x, y) => { Logger.Debug($"{x} {y}"); };
                    }
                }
            }
        }

        public ILogger Logger { get; set; }
    }
}