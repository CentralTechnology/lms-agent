using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam
{
    using System.Net;
    using System.Net.Sockets;
    using Administration;
    using Common.Constants;
    using Common.Extensions;
    using Factory;
    using NLog;

    public class VeeamManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool VeeamInstalled()
        {
            try
            {
                return Constants.VeeamApplicationName.IsApplictionInstalled();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                return false;
            }
        }

        public bool VeeamOnline()
        {
            IPAddress localhost = IPAddress.Parse("127.0.0.1");

            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(localhost, 9392);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error("Unable to contact the local Veeam server. Please make sure the services are started.");
                    Logger.Debug(ex);
                    return false;
                }
            }
        }

        public Version VeeamVersion()
        {
            Version veeamVersion = Constants.VeeamApplicationName.GetApplicationVersion();
            if (veeamVersion == null)
            {
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, string.Empty);
                return null;
            }

            SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, veeamVersion.ToString());
            return veeamVersion;
        }
    }
}
