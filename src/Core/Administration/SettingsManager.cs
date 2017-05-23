﻿namespace Core.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Abp;
    using Abp.Dependency;
    using Abp.Domain.Services;
    using Abp.Threading;
    using Castle.Core.Logging;
    using Common;
    using Common.Client;
    using Common.Enum;
    using Common.Extensions;
    using NLog;
    using NLog.Config;

    public class SettingsManager : DomainService, ISettingsManager
    {
        /// <inheritdoc />
        public SettingsData Update(SettingsData settings)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            Logger.Debug("Removing config.");
            config.Sections.Remove(LmsConstants.SettingsSection);

            Logger.Debug("Updating config.");
            config.Sections.Add(LmsConstants.SettingsSection, settings);

            Logger.Debug("Saving config.");
            config.Save();

            Logger.Debug("Config updated!");

            // added because SettingsData class cannot be serialized easily as it inherits from the Configuration
            var settingViewModel = new
            {
                settings.AccountId, settings.DeviceId, settings.Monitors
            };

            Logger.Debug($"New config: {settingViewModel.Dump()}");
            return settings;
        }

        /// <inheritdoc />
        public SettingsData Read()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SettingsData configData = (SettingsData) config.GetSection(LmsConstants.SettingsSection);

            return configData;
        }

        /// <inheritdoc />
        public LoggerLevel UpdateLoggerLevel(bool enableDebug)
        {
            Logger.Debug(enableDebug ? "Debug mode enabled." : "Debug mode disabled");
            SetLogLevel(enableDebug ? LogLevel.Debug : LogLevel.Info);

            return ReadLoggerLevel();
        }

        /// <inheritdoc />
        public LoggerLevel ReadLoggerLevel()
        {
            IList<LoggingRule> rules = LogManager.Configuration.LoggingRules;
            Regex validator = new Regex(LmsConstants.LoggerTarget);

            foreach (LoggingRule rule in rules.Where(r => validator.IsMatch(r.Targets[0].Name)))
            {
                if (rule.IsLoggingEnabledForLevel(LogLevel.Debug))
                {
                    return LoggerLevel.Debug;
                }

                if (rule.IsLoggingEnabledForLevel(LogLevel.Info))
                {
                    return LoggerLevel.Info;
                }

                if (rule.IsLoggingEnabledForLevel(LogLevel.Warn))
                {
                    return LoggerLevel.Warn;
                }

                if (rule.IsLoggingEnabledForLevel(LogLevel.Error))
                {
                    return LoggerLevel.Error;
                }

                if (rule.IsLoggingEnabledForLevel(LogLevel.Fatal))
                {
                    return LoggerLevel.Fatal;
                }
            }

            return LoggerLevel.Info;
        }

        /// <inheritdoc />
        public void Validate()
        {
            SettingsData config = Read();

            if (config.DeviceId == new Guid())
            {
                Logger.Warn("Centrastage device id is not set.");
#if DEBUG
                Guid deviceId = new Guid("5B7CB593-4BC1-24A0-EB59-76107F1E5255");

#else
var deviceId = GetDeviceId();
#endif

                config = Update(new SettingsData
                {
                    AccountId = config.AccountId,
                    DeviceId = deviceId,
                    Monitors = config.Monitors
                });

                Logger.Info($"Centrastage device id: {config.DeviceId}");
            }

            if (config.AccountId == 0)
            {
                Logger.Warn("Account id is not set.");
                int accountId = GetAccountId(config.DeviceId);

                config = Update(new SettingsData
                {
                    AccountId = accountId,
                    DeviceId = config.DeviceId,
                    Monitors = config.Monitors
                });

                Logger.Info($"Account id: {config.AccountId}");
            }

            if (Monitor.None.HasFlag(config.Monitors))
            {
                Logger.Warn("No actions are set to be monitored.");
                Monitor defaultMonitor = Monitor.Users;

                config = Update(new SettingsData
                {
                    AccountId = config.AccountId,
                    DeviceId = config.DeviceId,
                    Monitors = defaultMonitor
                });
            }

            Logger.Debug("Configuration is valid.");
        }

        /// <inheritdoc />
        public string GetClientVersion()
        {
            try
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to determine client version.");
                Logger.Debug(ex.ToString());
            }

            return string.Empty;
        }

        private int GetAccountId(Guid deviceId)
        {
            using (IDisposableDependencyObjectWrapper<ProfileClient> client = IocManager.Instance.ResolveAsDisposable<ProfileClient>())
            {
                // ReSharper disable once AccessToDisposedClosure
                return AsyncHelper.RunSync(() => client.Object.GetAccountByDeviceId(deviceId));
            }
        }

        private Guid GetDeviceId()
        {
            byte[] id;

            try
            {
                id = Encoding.UTF8.GetBytes(RegistryExtentions.GetRegistryValue(LmsConstants.DeviceIdKeyPath, LmsConstants.DeviceIdKeyName).ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to obtain the centrastage device id from the registry. Please manually enter this in the settings.json file.");
                Logger.Debug(ex.ToString());
                throw;
            }

            if (id == null)
            {
                throw new AbpException("Unable to obtain the centrastage device id from the registry. Please manually enter this in the settings.json file.");
            }

            string registryValue = Encoding.UTF8.GetString(id);
            Guid deviceId;
            bool valid = Guid.TryParse(registryValue, out deviceId);
            if (valid)
            {
                return deviceId;
            }

            throw new AbpException("Unable to validate the centrastage device id from the registry.");
        }

        private void SetLogLevel(LogLevel logLevel)
        {
            IList<LoggingRule> rules = LogManager.Configuration.LoggingRules;
            Regex validator = new Regex(LmsConstants.LoggerTarget);

            foreach (LoggingRule rule in rules.Where(r => validator.IsMatch(r.Targets[0].Name)))
            {
                rule.DisableLoggingForLevel(LogLevel.Debug);

                if (!rule.IsLoggingEnabledForLevel(logLevel))
                {
                    rule.EnableLoggingForLevel(logLevel);
                }
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}