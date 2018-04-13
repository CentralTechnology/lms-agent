namespace LMS.Core.StartUp
{
    using System;
    using Abp.Configuration;
    using Abp.Logging;
    using Configuration;
    using Extensions;
    using global::Hangfire.Server;
    using Managers;
    using Users.Managers;
    using Veeam.Managers;

    public class StartupManager : LMSManagerBase, IStartupManager
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly IVeeamManager _veeamManager;

        public StartupManager(
            IActiveDirectoryManager activeDirectoryManager,
            IVeeamManager veeamManager)
        {
            _activeDirectoryManager = activeDirectoryManager;
            _veeamManager = veeamManager;
        }

        public bool Init(PerformContext performContext)
        {
            Logger.Log(LogSeverity.Info, performContext, "Running startup process");

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
                bool userMonitorEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.UserMonitorEnabled);
                if (!userMonitorEnabled)
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
                bool veeamMonitorEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.VeeamMonitorEnabled);
                if (!veeamMonitorEnabled)
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
    }
}