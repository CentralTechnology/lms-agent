namespace LMS.Startup
{
    using System;
    using Abp.Configuration;
    using Autotask;
    using CentraStage;
    using Common.Managers;
    using Core.Configuration;
    using SharpRaven.Data;
    using Users.Managers;
    using Veeam.Managers;

    public class StartupManager : LMSManagerBase, IStartupManager
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly IAutotaskManager _autotaskManager;
        private readonly ICentraStageManager _centraStageManager;
        private readonly IVeeamManager _veeamManager;

        public StartupManager(
            ICentraStageManager centraStageManager,
            IAutotaskManager autotaskManager,
            IActiveDirectoryManager activeDirectoryManager,
            IVeeamManager veeamManager)
        {
            _centraStageManager = centraStageManager;
            _autotaskManager = autotaskManager;
            _activeDirectoryManager = activeDirectoryManager;
            _veeamManager = veeamManager;
        }

        public bool Init()
        {
            Logger.Info("Running initialisation...");
            Console.WriteLine(Environment.NewLine);

            try
            {
                ValidateCredentials();
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error(ex.Message);
            }

            Console.WriteLine(Environment.NewLine);

            try
            {
                bool monUsers = MonitorUsers();
                SettingManager.ChangeSettingForApplication(AppSettingNames.MonitorUsers, monUsers.ToString());
                Logger.Info(monUsers ? "Monitoring Users" : "Not Monitoring Users");
                Console.WriteLine(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            try
            {
                bool monVeeam = MonitorVeeam();
                SettingManager.ChangeSettingForApplication(AppSettingNames.MonitorVeeam, monVeeam.ToString());
                Logger.Info(monVeeam ? "Monitoring Veeam" : "Not Monitoring Veeam");
                Console.WriteLine(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            Logger.Info("************ Initialisation  Successful ************");
            return true;
        }

        public bool MonitorUsers()
        {
            Logger.Info("Dermining whether to monitor users...");

            try
            {
                bool userOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.UsersOverride);
                if (userOverride)
                {
                    Logger.Warn("User monitoring has been manually disabled.");
                    return false;
                }

                // check if a domain exists
                bool domainExists = _activeDirectoryManager.IsOnDomain();
                if (!domainExists)
                {
                    Logger.Warn("Check Domain: FAIL");
                    return false;
                }

                Logger.Info("Check Domain: OK");

                // check if this is a primary domain controller
                bool pdc = _activeDirectoryManager.IsPrimaryDomainController();
                if (!pdc)
                {
                    Logger.Warn("Check PDC: FAIL");

                    // check override is enabled
                    bool pdcOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.PrimaryDomainControllerOverride);
                    if (!pdcOverride)
                    {
                        Logger.Warn("Check PDC Override: FAIL");
                        return false;
                    }

                    Logger.Info("Check PDC Override: OK");
                }

                Logger.Info("Check PDC: OK");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug("Exception", ex);
                return false;
            }

            return true;
        }

        public bool MonitorVeeam()
        {
            Logger.Info("Dermining whether to monitor veeam...");
            try
            {
                bool veeamOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.VeeamOverride);
                if (veeamOverride)
                {
                    Logger.Warn("Veeam monitoring has been manually disabled.");
                    return false;
                }

                bool veeamInstalled = _veeamManager.IsInstalled();
                if (!veeamInstalled)
                {
                    Logger.Warn("Check Veeam Installed: FAIL");
                    return false;
                }

                Logger.Info("Check Veeam Installed: OK");

                // check the veeam version
                string veeamVersion = _veeamManager.GetVersion();
                if (veeamVersion == null)
                {
                    Logger.Warn("Check Veeam Version: FAIL");
                    return false;
                }

                // check if veeam is installed

                Logger.Info("Check Veeam Version: OK");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug("Exception", ex);
                return false;
            }

            return true;
        }

        public bool ValidateCredentials()
        {
            Logger.Info("Validating api credentials...");

            bool centraStage = _centraStageManager.IsValid();
            return centraStage && _autotaskManager.IsValid();
        }
    }
}