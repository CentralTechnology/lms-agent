namespace Core.Administration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using Common.Constants;
    using EntityFramework;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;

    public class SettingManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected RavenClient RavenClient;

        public SettingManager()
        {
            RavenClient = new RavenClient(Constants.SentryDSN);
        }

        public async Task ChangeSettingAsync(string name, string value)
        {
            await InsertOrUpdateSettingValueAsync(name, value);
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
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error("Unable to determine client version.");
                Logger.Debug(ex);
            }

            return string.Empty;
        }

        public Task<string> GetSettingValueAsync(string name)
        {
            return GetSettingValueInternalAsync(name);
        }

        /// <inheritdoc />
        public LoggerLevel ReadLoggerLevel()
        {
            //IList<LoggingRule> rules = LogManager.Configuration.LoggingRules;
            //var validator = new Regex(LmsConstants.LoggerTarget);

            //foreach (LoggingRule rule in rules.Where(r => validator.IsMatch(r.Targets[0].Name)))
            //{
            //    if (rule.IsLoggingEnabledForLevel(LogLevel.Debug))
            //    {
            //        return LoggerLevel.Debug;
            //    }

            //    if (rule.IsLoggingEnabledForLevel(LogLevel.Info))
            //    {
            //        return LoggerLevel.Info;
            //    }

            //    if (rule.IsLoggingEnabledForLevel(LogLevel.Warn))
            //    {
            //        return LoggerLevel.Warn;
            //    }

            //    if (rule.IsLoggingEnabledForLevel(LogLevel.Error))
            //    {
            //        return LoggerLevel.Error;
            //    }

            //    if (rule.IsLoggingEnabledForLevel(LogLevel.Fatal))
            //    {
            //        return LoggerLevel.Fatal;
            //    }
            //}

            return LoggerLevel.Info;
        }

        private void SetLogLevel(LogLevel logLevel)
        {
            //IList<LoggingRule> rules = LogManager.Configuration.LoggingRules;
            //var validator = new Regex(LmsConstants.LoggerTarget);

            //foreach (LoggingRule rule in rules.Where(r => validator.IsMatch(r.Targets[0].Name)))
            //{
            //    rule.DisableLoggingForLevel(LogLevel.Debug);

            //    if (!rule.IsLoggingEnabledForLevel(logLevel))
            //    {
            //        rule.EnableLoggingForLevel(logLevel);
            //    }
            //}

            //LogManager.ReconfigExistingLoggers();
        }

        /// <inheritdoc />
        public LoggerLevel UpdateLoggerLevel(bool enableDebug)
        {
            Logger.Debug(enableDebug ? "Debug mode enabled." : "Debug mode disabled");
            SetLogLevel(enableDebug ? LogLevel.Debug : LogLevel.Info);

            return ReadLoggerLevel();
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
            Setting setting = await GetSettingInternalAsync(name);

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
    }
}