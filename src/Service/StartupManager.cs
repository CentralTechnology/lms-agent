using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    using Core.Administration;
    using Core.Common.Extensions;
    using Core.Factory;
    using NLog;
    using OneTrueError.Client;

    public class StartupManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Init()
        {
            Logger.Info("Running initialisation...");

            try
            {
                bool monUsers = MonitorUsers();
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.MonitorUsers, monUsers.ToString());
                Logger.Info(monUsers ? "Monitoring Users" : "Not Monitoring Users");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                OneTrue.Report(ex);
            }

            Logger.Info("************ Initialisation  Successful ************");
        }

        private bool MonitorUsers()
        {
            Logger.Info("Dermining whether to monitor users...");

            // check if a domain exists
            var domainExists = DirectoryServicesFactory.DirectoryServicesManager().DomainExist();
            if (!domainExists)
            {
                Logger.Warn("Check Domain: FAIL");
                return false;
            }

            Logger.Info("Check Domain: OK");

            // check if this is a primary domain controller
            var pdc = DirectoryServicesFactory.DirectoryServicesManager().PrimaryDomainController();
            if (!pdc)
            {
                Logger.Warn("Check PDC: FAIL");

                // check override is enabled
                var pdcOverride = SettingFactory.SettingsManager().GetSettingValue<bool>(SettingNames.PrimaryDomainControllerOverride);
                if (!pdcOverride)
                {
                    Logger.Warn("Check PDC Override: FAIL");
                    return false;
                }

                Logger.Warn("Check PDC Override: OK");
            }

            Logger.Info("Check PDC: OK");

            return true;
        }
    }
}
