namespace LMS.Startup
{
    using System;
    using Abp.Configuration;
    using Abp.Dependency;
    using Autotask;
    using Castle.Core.Logging;
    using CentraStage;
    using Core.Common.Constants;
    using Core.Common.Extensions;
    using Core.Common.Helpers;
    using Core.Configuration;
    using Core.DirectoryServices;
    using SharpRaven;
    using SharpRaven.Data;
    using Veeam.Managers;

    public class StartupManager : ITransientDependency
    {
        public ILogger Logger { get; set; }
        protected SettingManager SettingManager;
        private readonly ICentraStageManager _centraStageManager;
        private readonly IAutotaskManager _autotaskManager;

        public StartupManager(SettingManager settingManager, ICentraStageManager centraStageManager, IAutotaskManager autotaskManager)
        {
            Logger = NullLogger.Instance;
            SettingManager = settingManager;
            _centraStageManager = centraStageManager;
            _autotaskManager = autotaskManager;
        }

        protected RavenClient RavenClient = Core.Sentry.RavenClient.Instance;

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

        private bool MonitorUsers()
        {
            Logger.Info("Dermining whether to monitor users...");
            var directoryServicesManager = new DirectoryServicesManager();
            try
            {
                bool userOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.UsersOverride);
                if (userOverride)
                {
                    Logger.Warn("User monitoring has been manually disabled.");
                    return false;
                }

                // check if a domain exists
                bool domainExists = directoryServicesManager.DomainExist();
                if (!domainExists)
                {
                    Logger.Warn("Check Domain: FAIL");
                    return false;
                }

                Logger.Info("Check Domain: OK");

                // check if this is a primary domain controller
                bool pdc = directoryServicesManager.PrimaryDomainController();
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

                    Logger.Warn("Check PDC Override: OK");
                }

                Logger.Info("Check PDC: OK");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug("Exception",ex);
                return false;
            }

            return true;
        }

        private bool MonitorVeeam()
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

                var veeamManager = new VeeamManager();

                // check if veeam is installed
                bool veeamInstalled = veeamManager.VeeamInstalled();
                if (!veeamInstalled)
                {
                    Logger.Warn("Check Veeam Installed: FAIL");
                    return false;
                }

                Logger.Info("Check Veeam Installed: OK");

                // check the veeam version
                string veeamVersion = veeamManager.VeeamVersion();
                if (veeamVersion == null)
                {
                    Logger.Warn("Check Veeam Version: FAIL");
                    return false;
                }

                Logger.Info("Check Veeam Version: OK");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug("Exception",ex);
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