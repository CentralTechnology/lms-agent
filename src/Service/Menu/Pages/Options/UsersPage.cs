namespace LMS.Menu.Pages.Options
{
    using Common.Extensions;
    using Core.Administration;
    using Core.Configuration;
    using EasyConsole;

    class UsersPage : MenuPage
    {
        private static readonly SettingManager SettingManager = new SettingManager();

        public UsersPage(Program program)
            : base("Users", program)
        {
            
            Menu.Add("Manual Override", () =>
            {
                var enabled = SettingManager.GetSettingValue<bool>(AppSettingNames.UsersOverride);
                SettingManager.ChangeSetting(AppSettingNames.UsersOverride, (!enabled).ToString());

                Output.WriteLine(!enabled ? "Enabled" : "Disabled");
                ActionComplete<UsersPage>();
            });

            Menu.Add("PDC Override: ", () =>
            {
                bool pdcOverride = SettingManager.GetSettingValue<bool>(AppSettingNames.PrimaryDomainControllerOverride);
                SettingManager.ChangeSetting(AppSettingNames.PrimaryDomainControllerOverride, (!pdcOverride).ToString());

                Output.WriteLine(!pdcOverride ? "Enabled" : "Disabled");
                ActionComplete<UsersPage>();
            });

        }
    }
}