namespace Core.Common.Client
{
    using System.Net;
    using Abp.Dependency;
    using Abp.WebApi.Client;
    using Castle.Core.Logging;
    using Microsoft.OData.Client;
    using Service;
    using Settings;

    public abstract class PortalClientBase : AbpWebApiClient
    {
        protected PortalContainer Container;
        protected string ServiceUrl;

        protected PortalClientBase()
        {
            Logger = NullLogger.Instance;
            SettingManager = IocManager.Instance.Resolve<ISettingManager>();

            ServiceUrl = SettingManager.GetServiceUrl();
            Container = new PortalContainer(new System.Uri(ServiceUrl));
        }

        public ILogger Logger { get; set; }
        public ISettingManager SettingManager { get; set; }

        protected void AddAccountIdHeader()
        {
            Container.BuildingRequest += (sender, e) => Container.OnBuildingRequest(sender, e, new Abp.NameValue
            {
                Name = "AccountId",
                Value = SettingManager.GetAccountId().ToString()
            });
        }

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