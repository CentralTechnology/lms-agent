namespace LMS.Core.StartUp
{
    using System;
    using System.Configuration;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Abp.Logging;
    using Configuration;
    using Extensions;
    using global::Hangfire.Server;
    using Managers;
    using Serilog;
    using Serilog.Events;
    using Users.Managers;
    using Veeam.Managers;

    public class StartupManager : DomainService, IStartupManager
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly IVeeamManager _veeamManager;

        private readonly ILogger _logger = Log.ForContext<StartupManager>();


        public StartupManager(
            IActiveDirectoryManager activeDirectoryManager,
            IVeeamManager veeamManager)
        {
            _activeDirectoryManager = activeDirectoryManager;
            _veeamManager = veeamManager;
        }

        public bool Init()
        {
            _logger.Information("Running startup process");

            try
            {
                bool monUsers = ShouldMonitorUsers();
                SettingManager.ChangeSettingForApplication(AppSettingNames.MonitorUsers, monUsers.ToString());

                _logger.Information(monUsers ? "Monitoring Users" : "Not Monitoring Users");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Debug(ex, ex.Message);
            }

            try
            {
                bool monVeeam = MonitorVeeam();
                SettingManager.ChangeSettingForApplication(AppSettingNames.MonitorVeeam, monVeeam.ToString());

                _logger.Information( monVeeam ? "Monitoring Veeam" : "Not Monitoring Veeam");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Debug(ex, ex.Message);
            }

            _logger.Information( "************ Initialisation  Successful ************");

            return true;
        }

        public bool ShouldMonitorUsers()
        {
            _logger.Information( "Dermining whether to monitor users...");

            try
            {
                bool userMonitorEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.UserMonitorEnabled);
                if (!userMonitorEnabled)
                {
                    _logger.Warning( "User monitoring has been manually disabled.");

                    return false;
                }

                // check if a domain exists
                bool domainExists = _activeDirectoryManager.IsOnDomain();
                if (!domainExists)
                {
                    _logger.Warning( "Check Domain: FAIL");

                    return false;
                }

                _logger.Information( "Check Domain: OK");

                // check if this is a primary domain controller
                bool pdc = _activeDirectoryManager.IsPrimaryDomainController();
                if (!pdc)
                {
                    _logger.Warning( "Check PDC: FAIL");

                    // check override is enabled
                    bool pdcOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.PrimaryDomainControllerOverride);
                    if (!pdcOverride)
                    {
                        _logger.Warning( "Check PDC Override: FAIL");
                        return false;
                    }

                    _logger.Information("Check PDC Override: OK");
                }

                _logger.Information( "Check PDC: OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Debug(ex, ex.Message);

                return false;
            }

            return true;
        }

        public bool MonitorVeeam()
        {
            _logger.Information("Dermining whether to monitor veeam...");

            try
            {
                bool veeamMonitorEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.VeeamMonitorEnabled);
                if (!veeamMonitorEnabled)
                {
                    _logger.Warning("Veeam monitoring has been manually disabled.");

                    return false;
                }

                bool veeamInstalled = _veeamManager.IsInstalled();
                if (!veeamInstalled)
                {
                    _logger.Warning( "Check Veeam Installed: FAIL");

                    return false;
                }

                _logger.Information( "Check Veeam Installed: OK");

                // check the veeam version
                Version veeamVersion = _veeamManager.GetInstalledVeeamVersion();
                if (veeamVersion == null)
                {
                    _logger.Warning( "Check Veeam Version: FAIL");

                    return false;
                }

                _logger.Information( "Check Veeam Version: OK");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Debug(ex, ex.Message);

                return false;
            }
        }
    }
}