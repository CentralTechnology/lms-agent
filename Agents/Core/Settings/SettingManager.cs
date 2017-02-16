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
    using Castle.Core.Logging;
    using Common.Extensions;
    using Newtonsoft.Json;
    using NLog;
    using NLog.Config;
    using ILogger = Castle.Core.Logging.ILogger;

    public class SettingManager : ISettingManager, ISingletonDependency, IShouldInitialize
    {
        private readonly string _settingFileName = "Settings.json";
        private Setting _settingFile;
        private string _settingFilePath;

        public SettingManager()
        {
            Logger = NullLogger.Instance;
        }

        public bool Debug { get; private set; }

        public ILogger Logger { get; set; }

        public void Create(long accoundId = 0)
        {
            // guard
            if (Exists())
            {
                Logger.Debug("Settings file already exists");
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
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("You do not have the required permissions to create the Settings file.");
                Logger.Debug("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }
            catch (Exception ex)
            {
                Logger.Error("A general error occurred while trying to create the Settings file.");
                Logger.Debug("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }

            Logger.Debug("Success.");
        }

        public bool Exists()
        {
            return File.Exists(_settingFilePath);
        }

        //public string GetSettingValue(string property)
        //{
        //    if (!Exists())
        //    {
        //        Logger.Warn("The Settings file does not exist.");
        //        Create();
        //        Logger.Warn("Please enter the correct Settings in settings.json file before running this application again.");
        //        Logger.Warn("The application will now quit.");
        //        Environment.Exit(1);
        //    }

        //    try
        //    {
        //        _settingFile = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(_settingFilePath));
        //    }
        //    catch (Exception)
        //    {
        //        Logger.Fatal("There was an error reading the Settings file.");
        //        throw;
        //    }

        //    return (string) _settingFile[property];
        //}

        public void SetDebug(bool value)
        {
            Logger.Info("Setting log level to Debug.");

            Debug = value;

            SetLogLevel(LogLevel.Debug, "file");
        }

        public Guid GetDeviceId()
        {
            Logger.Debug("Attempting to read the CentraStage DeviceID from the Settings file.");
            var id = _settingFile.DeviceId;
            if (id != null)
            {
                Logger.DebugFormat("CentraStage DeviceId is currently set to: {0}", id);
                return (Guid) id;
            }

            Logger.Debug("The CentraStage DeviceID is not stored in the Settings file.");
            Logger.Debug("Attempting to read the CentraStage DeviceID from the registry.");

            var deviceId = GetDeviceIdFromRegistry();

            Logger.Debug("CentraStage DeviceID has been found!");
            Logger.Debug("Attempting to set the CentraStage DeviceID in the Settings file.");

            SetDeviceId(deviceId);

            Logger.DebugFormat("CentraStage DeviceId is currently set to: {0}", deviceId);
            return deviceId;
        }

        public long GetAccountId()
        {
            var id = _settingFile.AccountId;
            try
            {
                if (id == 0)
                {
                    throw new Exception("Autotask AccountID is currently set to 0. Please update this in the Settings.json file.");
                }
                if (id == null)
                {
                    throw new NullReferenceException("Autotask AccountID is not currently set. Please update this in the Settings.json file");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.DebugFormat("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }

            return (long)id;
        }

        public string GetServiceUrl()
        {
            return _settingFile.ServiceUrl;
        }

        public void Initialize()
        {
            _settingFilePath = AppDomain.CurrentDomain.BaseDirectory + _settingFileName;
        }

        private Guid GetDeviceIdFromRegistry()
        {
            byte[] id = (byte[]) RegistryExtentions.GetRegistryValue(Setting.DeviceIdKeyPath, Setting.DeviceIdKeyName);
            if (id == null)
            {
                throw new NullReferenceException("Unable to obtain the CentraStage DeviceID from the registry. Please manually enter this in the Settings.json file before starting the service again.");
            }

            string registryValue = Encoding.UTF8.GetString(id);
            Guid deviceId;
            bool valid = Guid.TryParse(registryValue, out deviceId);
            if (valid)
            {
                return deviceId;
            }

            throw new FormatException("Unable to validate the CentraStage DeviceID from the registry.");
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
                _settingFile = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(_settingFilePath));
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("You do not have the required permissions to read the Settings file.");
                Logger.Debug("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }
            catch (Exception ex)
            {
                Logger.Error("A general error occurred while trying to read the Settings file.");
                Logger.Debug("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }
        }

        private void SetDeviceId(Guid deviceId)
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
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("You do not have the required permissions to update the Settings file.");
                Logger.Debug("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }
            catch (Exception ex)
            {
                Logger.Error("A general error occurred while trying to update the Settings file.");
                Logger.Debug("Exception: ", ex);
                Environment.Exit(ex.HResult);
            }

            Logger.Debug("Success.");
        }
    }
}