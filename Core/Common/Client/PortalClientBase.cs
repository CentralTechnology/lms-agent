namespace LicenseMonitoringSystem.Core.Common.Client
{
    using System.Net;
    using Abp.Web.Security.AntiForgery;
    using Abp.WebApi.Client;
    using Castle.Core.Logging;
    using Microsoft.OData.Client;
    using Settings;

    public abstract class PortalClientBase : AbpWebApiClient
    {
        protected PortalClientBase(SettingManager settingManager, IAbpAntiForgeryManager antiForgeryManager)
        {
            Logger = NullLogger.Instance;
            SettingManager = settingManager;

            RequestHeaders.Add(new Abp.NameValue
            {
                Name = "X-XSRF-TOKEN",
                Value = antiForgeryManager.GenerateToken()
            });

            RequestHeaders.Add(new Abp.NameValue
            {
                Name = "Authorization",
                Value = "Device" + SettingManager.GetDeviceId()
            });
        }

        public ILogger Logger { get; set; }
        public SettingManager SettingManager { get; set; }

        public void HandleResponse(DataServiceResponse response)
        {
            foreach (var operationResponse in response)
            {
                if (operationResponse.StatusCode == (int) HttpStatusCode.BadRequest)
                {
                    Logger.ErrorFormat($"Status Code: {operationResponse.StatusCode} \t Reason: {operationResponse.Error}");
                }
            }
        }
    }
}