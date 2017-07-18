namespace Service
{
    using System;
    using Abp;
    using Core.Administration;
    using Core.Common.Constants;
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
            Console.WriteLine(Environment.NewLine);

            try
            {
                bool monUsers = MonitorUsers();
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.MonitorUsers, monUsers.ToString());
                Logger.Info(monUsers ? "Monitoring Users" : "Not Monitoring Users");
                Console.WriteLine(Environment.NewLine);

                bool monVeeam = MonitorVeeam();
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.MonitorVeeam, monVeeam.ToString());
                Logger.Info(monUsers ? "Monitoring Veeam" : "Not Monitoring Veeam");
                Console.WriteLine(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                OneTrue.Report(ex);
            }

            try
            {
                ValidateApiCredentials();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error("************ Initialisation  Failed ************");
                Logger.Debug(ex);
                OneTrue.Report(ex);
                throw;
            }

            Logger.Info("************ Initialisation  Successful ************");
        }

        private bool MonitorUsers()
        {
            Logger.Info("Dermining whether to monitor users...");

            // check if a domain exists
            bool domainExists = DirectoryServicesFactory.DirectoryServicesManager().DomainExist();
            if (!domainExists)
            {
                Logger.Warn("Check Domain: FAIL");
                return false;
            }

            Logger.Info("Check Domain: OK");

            // check if this is a primary domain controller
            bool pdc = DirectoryServicesFactory.DirectoryServicesManager().PrimaryDomainController();
            if (!pdc)
            {
                Logger.Warn("Check PDC: FAIL");

                // check override is enabled
                bool pdcOverride = SettingFactory.SettingsManager().GetSettingValue<bool>(SettingNames.PrimaryDomainControllerOverride);
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

        private bool MonitorVeeam()
        {
            Logger.Info("Dermining whether to monitor veeam...");

            // check if veeam is installed
            bool veeamInstalled = VeeamFactory.VeeamManager().VeeamInstalled();
            if (!veeamInstalled)
            {
                Logger.Warn("Check Veeam Installed: FAIL");
                return false;
            }

            Logger.Info("Check Veeam Installed: OK");

            // check veeam is online
            bool veeamOnline = VeeamFactory.VeeamManager().VeeamOnline();
            if (!veeamOnline)
            {
                Logger.Warn("Check Veeam Online: FAIL");
                return false;
            }

            Logger.Info("Check Veeam Online: OK");

            // check the veeam version
            Version veeamVersion = VeeamFactory.VeeamManager().VeeamVersion();
            if (veeamVersion == null)
            {
                Logger.Warn("Check Veeam Version: FAIL");
                return false;
            }

            Logger.Warn("Check Veeam Version: OK");
            return true;
        }

        private void ValidateApiCredentials()
        {
            // get the centrastage device id
            Logger.Info("Getting the centrastage device id...");

            var deviceId = Constants.CentraStage.GetCentrastageId();
            if (deviceId == null)
            {
                Logger.Warn("Get centrastage device id: FAIL");
                throw new AbpException("Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");
            }

            Logger.Info($"Get centrastage device id: {deviceId}");
            SettingFactory.SettingsManager().ChangeSetting(SettingNames.CentrastageDeviceId, deviceId.ToString());

            // get the autotask account id
            Logger.Info("Getting the autotask account id id...");

            var accountId = SettingFactory.SettingsManager().GetAccountId((Guid) deviceId);

            if (accountId == 0)
            {
                Logger.Warn("Get autotask account id: FAIL");
                throw new AbpException("Failed to get the autotask account id from the api. This application cannot work without the autotask account id. Please enter it manually through the menu system.");
            }

            Logger.Info($"Get autotask account id: {accountId}");
            SettingFactory.SettingsManager().ChangeSetting(SettingNames.AutotaskAccountId, accountId.ToString());
        }
    }
}