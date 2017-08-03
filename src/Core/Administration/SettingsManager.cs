namespace Core.Administration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
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