namespace LicenseMonitoringSystem.Core.Common.Service
{
    using System;
    using Abp;
    using Abp.Dependency;
    using Abp.Web.Security.AntiForgery;
    using Actions;
    using Microsoft.OData.Client;
    using Settings;

    public class PortalContainer : Container
    {
        private readonly IAbpAntiForgeryManager _antiForgeryManager;
        private readonly ISettingManager _settingManager;

        public PortalContainer(Uri serviceRoot)
            : base(serviceRoot)
        {
            _antiForgeryManager = IocManager.Instance.Resolve<IAbpAntiForgeryManager>();
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
        }

        public void OnBuildingRequest(object sender, BuildingRequestEventArgs e, NameValue header = null)
        {
            e.Headers.Clear();

            if (header != null)
            {
                e.Headers.Add(header.Name, header.Value);
            }

            e.Headers.Add("X-XSRF-TOKEN", _antiForgeryManager.GenerateToken());

            e.Headers.Add("Authorization", "Device " + _settingManager.GetDeviceId());
        }
    }
}