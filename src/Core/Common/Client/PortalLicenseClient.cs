namespace Core.Common.Client
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
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
        private readonly ISettingsManager _settingsManager;
        private readonly PortalClient _portalClient;
        public DefaultLicenseClientSettings()
        {
            _settingsManager = IocManager.Instance.Resolve<ISettingsManager>();
            _portalClient = IocManager.Instance.Resolve<PortalClient>();

            Logger = NullLogger.Instance;

            BaseUri = new Uri(LmsConstants.DefaultServiceUrl);
            var accountId = _settingsManager.Read().AccountId.ToString();
            var deviceId = _settingsManager.Read().DeviceId.ToString();
            var token = AsyncHelper.RunSync(() => _portalClient.GetTokenCookie());


            BeforeRequest += br =>
            {
                br.ShouldIncludeErrorDetail();
                br.Headers.Clear();

                // ReSharper disable once AccessToDisposedClosure
                br.Headers.Add("AccountId", accountId);
                // ReSharper disable once AccessToDisposedClosure
                br.Headers.Add("XSRF-TOKEN", token);
                // ReSharper disable once AccessToDisposedClosure
                br.Headers.Authorization = new AuthenticationHeaderValue("Device", deviceId);
            };

            if (_settingsManager.ReadLoggerLevel() == LoggerLevel.Debug)
            {
                OnTrace += (x, y) => { Logger.Debug($"{x} {y}"); };
            }
        }

        public ILogger Logger { get; set; }
    }
}