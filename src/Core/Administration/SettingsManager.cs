﻿namespace Core.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Abp;
    using Abp.Dependency;
    using Abp.Domain.Services;
    using Abp.Threading;
    using Castle.Core.Logging;
    using Common;
    using Common.Client;
    using Common.Enum;
    using Common.Extensions;
    using EntityFramework;
    using Factory;
    using NLog;
    using NLog.Config;
    using Abp.Extensions;
    using System.Data.Entity.Migrations;

    public class SettingManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Task<string> GetSettingValueAsync(string name)
        {
            return GetSettingValueInternalAsync(name);
        }

        public async Task ChangeSettingForApplicationAsync(string name, string value)
        {
            await InsertOrUpdateSettingValueAsync(name, value);
        }

        #region Private methods

        private Task<string> GetSettingValueInternalAsync(string name)
        {
            using (var context = new AgentDbContext())
            {
                return context.Settings.Where(s => s.Name.Equals(name)).Select(s => s.Value).FirstOrDefaultAsync();
            }
        }

        private Task<Setting> GetSettingInternalAsync(string name)
        {
            using (var context = new AgentDbContext())
            {
                return context.Settings.Where(s => s.Name.Equals(name)).FirstOrDefaultAsync();
            }
        }

        private async Task<Setting> InsertOrUpdateSettingValueAsync(string name, string value)
        {
            var setting = await GetSettingInternalAsync(name);

            using (var context = new AgentDbContext())
            {
                // if its not stored in the database, then insert it
                if (setting == null)
                {
                    setting = new Setting(name, value);
                }
                else
                {
                    setting.Value = value;
                }

                context.Settings.AddOrUpdate(setting);
                await context.SaveChangesAsync();
            }

            return setting;
        }

        #endregion



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
                settings.AccountId,
                settings.DeviceId,
                settings.Monitors
            };

            Logger.Debug($"New config: {settingViewModel.Dump()}");
            return settings;
        }

        /// <inheritdoc />
        public SettingsData Read()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configData = (SettingsData)config.GetSection(LmsConstants.SettingsSection);

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
            var validator = new Regex(LmsConstants.LoggerTarget);

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
                var deviceId = new Guid("5B7CB593-4BC1-24A0-EB59-76107F1E5255");

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
                var defaultMonitor = Monitor.Users;

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
            return AsyncHelper.RunSync(() => ClientFactory.ProfileClient().GetAccountByDeviceId(deviceId));
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
            var validator = new Regex(LmsConstants.LoggerTarget);

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