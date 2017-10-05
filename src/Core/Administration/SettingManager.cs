namespace Core.Administration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using EntityFramework;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;

    public class SettingManager
    {
        protected RavenClient RavenClient;

        public SettingManager()
        {
            RavenClient = Sentry.RavenClient.Instance;
        }

        public async Task ChangeSettingAsync(string name, string value)
        {
            await InsertOrUpdateSettingValueAsync(name, value);
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