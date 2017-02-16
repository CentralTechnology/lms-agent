namespace LicenseMonitoringSystem.Core.Common.Client
{
    using System;
    using System.Linq;
    using Abp.Web.Security.AntiForgery;
    using Actions;
    using Microsoft.OData.Client;
    using Portal.Common.Enums;
    using Portal.License.User;
    using Settings;
    using Tools;

    public class PortalClient : PortalClientBase, IPortalClient
    {
        private Container _container;
        private string _serviceUrl;

        public PortalClient(SettingManager settingManager, IAbpAntiForgeryManager antiForgeryManager)
            : base(
                settingManager,
                antiForgeryManager
            )
        {
        }

        public LicenseUserUpload Get(int id)
        {
            return _container.LicenseUser.SingleOrDefault(u => u.Id.Equals(id));
        }

        public int GetId(Guid deviceId)
        {
            return _container.LicenseUser.Id(deviceId).GetValue();
        }

        public int GetUploadId(DateTime? date)
        {
            return _container.LicenseUser.UploadId(date).GetValue();
        }

        public void Post(LicenseUserUpload entity)
        {
            _container.AddToLicenseUser(entity);
            var serviceResponse = _container.SaveChanges();
            HandleResponse(serviceResponse);
        }

        public void Put(int id, LicenseUserUpload entity)
        {
            _container.UpdateObject(entity);
            var response = _container.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
            HandleResponse(response);
        }

        public CallInStatus GetStatus(Guid deviceId)
        {
            return _container.LicenseUser.Status(deviceId).GetValue();
        }

        public void Initialize()
        {
            _serviceUrl = SettingManager.GetServiceUrl();
            _container = new Container(new Uri(_serviceUrl));
        }

        public int? GetAccountId(Guid deviceId)
        {
            return _container.DeviceProfile.AccountId(deviceId).GetValue();
        }
    }
}