namespace Core.Migrations
{
    using System;
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
            context.Settings.Add(new Setting(SettingNames.AutotaskAccountId, default(int).ToString()));
            context.Settings.Add(new Setting(SettingNames.CentrastageDeviceId, default(Guid).ToString()));
            context.Settings.Add(new Setting(SettingNames.MonitorUsers, false.ToString()));
            context.Settings.Add(new Setting(SettingNames.MonitorVeeam, false.ToString()));
            context.Settings.Add(new Setting(SettingNames.PrimaryDomainControllerOverride, false.ToString()));
            context.Settings.Add(new Setting(SettingNames.VeeamOverride, false.ToString()));
            context.Settings.Add(new Setting(SettingNames.UsersOverride, false.ToString()));
        }
    }
}