namespace LMS.Startup
{
    using System;
    using Abp.Configuration;
    using Abp.Logging;
    using Autotask;
    using CentraStage;
    using Common.Extensions;
    using Common.Managers;
    using Core.Configuration;
    using global::Hangfire.Server;
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

        public bool Init(PerformContext performContext)
        {
            Logger.Log(LogSeverity.Info, performContext, "Running startup process");

            try
            {
                ValidateCredentials(performContext);
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
            }

            try
            {
                bool monUsers = ShouldMonitorUsers(performContext);
                SettingManager.ChangeSettingForApplication(AppSettingNames.MonitorUsers, monUsers.ToString());

                Logger.Log(LogSeverity.Info, performContext, monUsers ? "Monitoring Users" : "Not Monitoring Users");
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
            }

            try
            {
                bool monVeeam = MonitorVeeam(performContext);
                SettingManager.ChangeSettingForApplication(AppSettingNames.MonitorVeeam, monVeeam.ToString());

                Logger.Log(LogSeverity.Info, performContext, monVeeam ? "Monitoring Veeam" : "Not Monitoring Veeam");
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
            }

            Logger.Log(LogSeverity.Info, performContext, "************ Initialisation  Successful ************");

            return true;
        }

        public bool ShouldMonitorUsers(PerformContext performContext)
        {
            Logger.Log(LogSeverity.Info, performContext, "Dermining whether to monitor users...");

            try
            {
                bool userOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.UsersOverride);
                if (userOverride)
                {
                    Logger.Log(LogSeverity.Warn, performContext, "User monitoring has been manually disabled.");

                    return false;
                }

                // check if a domain exists
                bool domainExists = _activeDirectoryManager.IsOnDomain(performContext);
                if (!domainExists)
                {
                    Logger.Log(LogSeverity.Warn, performContext, "Check Domain: FAIL");

                    return false;
                }

                Logger.Log(LogSeverity.Info, performContext, "Check Domain: OK");

                // check if this is a primary domain controller
                bool pdc = _activeDirectoryManager.IsPrimaryDomainController(performContext);
                if (!pdc)
                {
                    Logger.Log(LogSeverity.Warn, performContext, "Check PDC: FAIL");

                    // check override is enabled
                    bool pdcOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.PrimaryDomainControllerOverride);
                    if (!pdcOverride)
                    {
                        Logger.Log(LogSeverity.Warn, performContext, "Check PDC Override: FAIL");
                        return false;
                    }

                    Logger.Log(LogSeverity.Info, performContext, "Check PDC Override: OK");
                }

                Logger.Log(LogSeverity.Info, performContext, "Check PDC: OK");
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
                Logger.Log(LogSeverity.Debug, performContext, ex.Message, ex);

                return false;
            }

            return true;
        }

        public bool MonitorVeeam(PerformContext performContext)
        {
            Logger.Log(LogSeverity.Info, performContext, "Dermining whether to monitor veeam...");

            try
            {
                bool veeamOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.VeeamOverride);
                if (veeamOverride)
                {
                    Logger.Log(LogSeverity.Warn, performContext, "Veeam monitoring has been manually disabled.");

                    return false;
                }

                bool veeamInstalled = _veeamManager.IsInstalled(performContext);
                if (!veeamInstalled)
                {
                    Logger.Log(LogSeverity.Warn, performContext, "Check Veeam Installed: FAIL");

                    return false;
                }

                Logger.Log(LogSeverity.Info, performContext, "Check Veeam Installed: OK");

                // check the veeam version
                string veeamVersion = _veeamManager.GetVersion();
                if (veeamVersion == null)
                {
                    Logger.Log(LogSeverity.Warn, performContext, "Check Veeam Version: FAIL");

                    return false;
                }

                Logger.Log(LogSeverity.Info, performContext, "Check Veeam Version: OK");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
                Logger.Log(LogSeverity.Info, performContext, ex.Message, ex);

                return false;
            }
        }

        public bool ValidateCredentials(PerformContext performContext)
        {
            Logger.Log(LogSeverity.Info, performContext, "Validating api credentials...");

            bool centraStage = _centraStageManager.IsValid(performContext);
            return centraStage && _autotaskManager.IsValid(performContext);
        }
    }
}