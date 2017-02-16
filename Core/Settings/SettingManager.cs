namespace LicenseMonitoringSystem.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Abp;
    using Abp.Dependency;
    using Abp.Extensions;
    using Common.Client;
    using Common.Extensions;
    using Newtonsoft.Json;
    using NLog;
    using NLog.Config;
    using ILogger = Castle.Core.Logging.ILogger;
    using NullLogger = Castle.Core.Logging.NullLogger;

    public class SettingManager : ISettingManager, ISingletonDependency, IShouldInitialize
    {
        private readonly string _settingFileName = "Settings.json";
        private Setting _settingFile;
        private string _settingFilePath;

        public Guid DeviceId { get; set; }
        public int AccountId { get; set; }


        public SettingManager()
        {
            Logger = NullLogger.Instance;
        }

        public bool Debug { get; private set; }

        public ILogger Logger { get; set; }

        public void Create(int accoundId = 0)
        {
            // guard
            if (Exists())
            {
                return;
            }

            // create new file and set account id if possible
            _settingFile = new Setting();
            if (accoundId != 0)
            {
                _settingFile.AccountId = accoundId;
            }

            string settings = JsonConvert.SerializeObject(_settingFile, Formatting.Indented);

            try
            {
                Logger.Debug("Creating the Settings file.");
                File.WriteAllText(_settingFilePath, settings);
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Error("You do not have the required permissions to create the Settings file.");
                throw;
            }
            catch (Exception)
            {
                Logger.Error("A general error occurred while trying to create the Settings file.");
                throw;
            }

            Logger.Debug("Success.");
        }

        public bool Exists()
        {
            var exists = File.Exists(_settingFilePath);
            Logger.Debug(exists ? "Settings.json file found" : "Settings.json file not found");

            return exists;
        }

        public void SetDebug(bool value)
        {
            Logger.Info(value ? "Turning on Debug" : "Turning off Debug");

            Debug = value;

            SetLogLevel(value ? LogLevel.Debug : LogLevel.Error, "file");
        }

        private Guid? GetDeviceIdFromSettingFile()
        {
            Logger.Debug("Attempting to read the CentraStage DeviceID from the Settings file.");
            var id = _settingFile?.DeviceId;

            if (id != null)
            {
                Logger.DebugFormat("CentraStage device id is currently set to: {0}", id);
            }
            else
            {
                Logger.Debug("CentraStage device id is not currently set");
            }

            return id;
        }

        public int? GetAccountId(Guid deviceId)
        {
            var acctId = _settingFile?.AccountId;
            if (acctId != null)
            {
                return acctId;
            }

            using (var client = IocManager.Instance.ResolveAsDisposable<PortalClient>())
            {
                return client.Object.GetAccountId(deviceId);
            }
        }

        public string GetServiceUrl()
        {
            return _settingFile?.ServiceUrl;
        }

        public void SetAccountId(int accountId)
        {
            GetSettings();

            _settingFile.AccountId = accountId;

            UpdateSettings();
        }

        public void FirstRun()
        {
            // check settings file is present
            var exists = Exists();

            if (!exists)
            {
                Create();
            }

            if (!_settingFile.ResetOnStartUp)
            {
                return;
            }

            // gets/sets device id
            var deviceId = GetDeviceId(true);

            if (deviceId == null)
            {
                throw new AbpException("Unable to obtain the centrastage device id. Execution cannot continue. Please update the settings.json file with the correct centrastage device id.");
            }

            SetDeviceId(deviceId);

            // gets/sets account id
            var accountId = GetAccountId((Guid)deviceId);

            if (accountId == null)
            {
                throw new AbpException("Unable to obtain the autotask account id. Execution cannot continue. Please update the settings.json file with the correct autotask acccount id.");
            }

            SetAccountId((int)accountId);

            ResetOnStartUp(false);
        }

        public int? GetAccountId()
        {
            return _settingFile?.AccountId;
        }

        public Guid GetDeviceId(bool registry)
        {
            Guid deviceId;
            if (registry)
            {
                deviceId = GetDeviceIdFromRegistry();
            }
            else
            {
                var id = GetDeviceIdFromSettingFile();
                var valid = Guid.TryParse(id.ToString(), out deviceId);
                if (!valid)
                {
                    throw new AbpException("Invalid centrastage device id.");
                }
            }

            return deviceId;
        }

        public void Initialize()
        {
            _settingFilePath = AppDomain.CurrentDomain.BaseDirectory + _settingFileName;
        }

        private Guid GetDeviceIdFromRegistry()
        {
            byte[] id = Encoding.UTF8.GetBytes(RegistryExtentions.GetRegistryValue(Setting.DeviceIdKeyPath, Setting.DeviceIdKeyName).ToString());
            if (id == null)
            {
                throw new AbpException("Unable to obtain the centrastage device id from the registry. Please manually enter this in the settings.json file.");
            }

            var registryValue = Encoding.UTF8.GetString(id);
            Guid deviceId;
            bool valid = Guid.TryParse(registryValue, out deviceId);
            if (valid)
            {
                return deviceId;
            }

            throw new AbpException("Unable to validate the centrastage device id from the registry.");
        }

        public List<Monitor> GetMonitors()
        {
            List<Monitor> monitors = new List<Monitor>();
            foreach (var monitor in _settingFile.Monitor)
            {
                monitors.Add(monitor.ToEnum<Monitor>());
            }

            return monitors;
        }

        private void GetSettings()
        {
            try
            {
                Logger.Debug("Reading the Settings file.");

                bool exists = Exists();
                if (exists)
                {
                    _settingFile = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(_settingFilePath));
                }
                else
                {
                    Create();
                }
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Error("You do not have the required permissions to read the Settings file.");
                throw;
            }
            catch (Exception)
            {
                Logger.Error("A general error occurred while trying to read the Settings file.");
                throw;
            }
        }

        public void SetDeviceId(Guid deviceId)
        {
            GetSettings();

            _settingFile.DeviceId = deviceId;

            UpdateSettings();
        }

        private void SetLogLevel(LogLevel logLevel, string regex)
        {
            IList<LoggingRule> rules = LogManager.Configuration.LoggingRules;
            Regex validator = new Regex(regex);

            foreach (var rule in rules.Where(r => validator.IsMatch(r.Targets[0].Name)))
            {
                if (!rule.IsLoggingEnabledForLevel(logLevel))
                {
                    rule.EnableLoggingForLevel(logLevel);
                }
            }
        }

        private void UpdateSettings()
        {
            string settings = JsonConvert.SerializeObject(_settingFile, Formatting.Indented);

            try
            {
                Logger.Debug("Updating the Settings file...");
                File.WriteAllText(_settingFilePath, settings);
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Error("You do not have the required permissions to update the Settings file.");
            }
            catch (Exception)
            {
                Logger.Error("A general error occurred while trying to update the Settings file.");
            }

            Logger.Debug("Success.");
        }

        public void ResetOnStartUp(bool value)
        {
            GetSettings();

            _settingFile.ResetOnStartUp = value;

            UpdateSettings();
        }

        public Guid? GetDeviceId()
        {
            return GetDeviceIdFromSettingFile();
        }

        public void ClearCache()
        {
            GetSettings();

            _settingFile.AccountId = null;
            _settingFile.DeviceId = null;

            UpdateSettings();
        }
    }
}