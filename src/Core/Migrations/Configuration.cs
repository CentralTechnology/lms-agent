using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Migrations
{
    using System.Data.Entity.Migrations;
    using Administration;
    using EntityFramework;

    public sealed class Configuration : DbMigrationsConfiguration<AgentDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationDataLossAllowed = false;
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AgentDbContext context)
        {
            // default configuration
            context.Settings.Add(new Setting(SettingNames.AutotaskAccountId, string.Empty));
            context.Settings.Add(new Setting(SettingNames.CentrastageDeviceId, string.Empty));
            context.Settings.Add(new Setting(SettingNames.DebugMode, false.ToString()));
            context.Settings.Add(new Setting(SettingNames.MonitorUsers, false.ToString()));
            context.Settings.Add(new Setting(SettingNames.MonitorVeeam, false.ToString()));
        }
    }
}
