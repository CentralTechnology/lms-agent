namespace Service
{
    using System;
    using Abp;
    using Abp.Threading;
    using Core.Administration;
    using Core.Common.Client;
    using Core.Common.Constants;
    using Core.Common.Extensions;
    using Core.Factory;
    using Core.Veeam;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;

    public class StartupManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected ProfileClient ProfileClient = new ProfileClient();
        protected RavenClient RavenClient = Core.Sentry.RavenClient.New();
        protected SettingManager SettingManager = new SettingManager();
        protected VeeamManager VeeamManager = new VeeamManager();

        public void Init()
        {
            Logger.Info("Running initialisation...");
            Console.WriteLine(Environment.NewLine);

            try
            {
                bool monUsers = MonitorUsers();
                SettingManager.ChangeSetting(SettingNames.MonitorUsers, monUsers.ToString());
                Logger.Info(monUsers ? "Monitoring Users" : "Not Monitoring Users");
                Console.WriteLine(Environment.NewLine);

                bool monVeeam = MonitorVeeam();
                SettingManager.ChangeSetting(SettingNames.MonitorVeeam, monVeeam.ToString());
                Logger.Info(monVeeam ? "Monitoring Veeam" : "Not Monitoring Veeam");
                Console.WriteLine(Environment.NewLine);
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error(ex.Message);
            }

            try
            {
                ValidateApiCredentials();
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error(ex.Message);
                Logger.Error("************ Initialisation  Failed ************");
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
            bool veeamInstalled = VeeamManager.VeeamInstalled();
            if (!veeamInstalled)
            {
                Logger.Warn("Check Veeam Installed: FAIL");
                return false;
            }

            Logger.Info("Check Veeam Installed: OK");

            // check veeam is online
            bool veeamOnline = VeeamManager.VeeamOnline();
            if (!veeamOnline)
            {
                Logger.Warn("Check Veeam Online: FAIL");
                return false;
            }

            Logger.Info("Check Veeam Online: OK");

            // check the veeam version
            string veeamVersion = VeeamManager.VeeamVersion();
            if (veeamVersion == null)
            {
                Logger.Warn("Check Veeam Version: FAIL");
                return false;
            }

            Logger.Info("Check Veeam Version: OK");
            return true;
        }

        private void ValidateApiCredentials()
        {
            // get the centrastage device id
            Logger.Info("Validating api credentials...");

            Guid deviceId;
            var storedDevice = SettingManager.GetSettingValue<Guid>(SettingNames.CentrastageDeviceId);
            if (storedDevice == default(Guid))
            {
                Guid? reportedDevice = Constants.CentraStage.GetCentrastageId();

                if (reportedDevice == null)
                {
                    Logger.Warn("Check Centrastage: FAIL");
                    throw new AbpException("Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");
                }

                SettingManager.ChangeSetting(SettingNames.CentrastageDeviceId, reportedDevice.ToString());
                deviceId = reportedDevice.To<Guid>();
            }
            else
            {
                deviceId = storedDevice;
            }

            Logger.Info("Check Centrastage: OK");
            Logger.Info($"Device: {deviceId}");

            int accountId;
            int storedAccount = SettingManager.GetSettingValue<int>(SettingNames.AutotaskAccountId);
            if (storedAccount == default(int))
            {
                int reportedAccount = AsyncHelper.RunSync(() => ProfileClient.GetAccountByDeviceId(deviceId));

                if (reportedAccount == default(int))
                {
                    Logger.Warn("Check Account: FAIL");
                    throw new AbpException("Failed to get the autotask account id from the api. This application cannot work without the autotask account id. Please enter it manually through the menu system.");
                }

                SettingFactory.SettingsManager().ChangeSetting(SettingNames.AutotaskAccountId, reportedAccount.ToString());
                accountId = reportedAccount.To<int>();
            }
            else
            {
                accountId = storedAccount;
            }

            Logger.Info("Check Account: OK");
            Logger.Info($"Account: {accountId}");
        }
    }
}