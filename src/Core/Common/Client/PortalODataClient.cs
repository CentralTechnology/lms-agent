using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Client
{
    using Helpers;
    using NLog;
    using Simple.OData.Client;

    public class PortalODataClient
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static ODataClient New()
        {
            var settings = new PortalODataClientSettings();

            settings.BeforeRequest += br =>
            {
                br.Headers.Add("AccountId", SettingManagerHelper.AccountId.ToString());
                br.Headers.Add("XSRF-TOKEN", SettingManagerHelper.Token);
                br.Headers.Add("Authorization", $"Device {SettingManagerHelper.DeviceId.ToString("D").ToUpper()}");
            };

            return new ODataClient(settings);
        }
    }
}
