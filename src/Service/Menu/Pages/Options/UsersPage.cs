namespace Service.Menu.Pages.Options
{
    using System;
    using Core.Administration;
    using Core.Common.Extensions;
    using EasyConsole;

    class UsersPage : MenuPage
    {
        private static readonly SettingManager SettingManager = new SettingManager();

        public UsersPage(Program program)
            : base("Users", program)
        {
            
            Menu.Add("Manual Override", () =>
            {
                var enabled = SettingManager.GetSettingValue<bool>(SettingNames.UsersOverride);
                SettingManager.ChangeSetting(SettingNames.UsersOverride, (!enabled).ToString());

                Output.WriteLine(!enabled ? "Enabled" : "Disabled");
                ActionComplete<UsersPage>();
            });

            Menu.Add("PDC Override: ", () =>
            {
                bool pdcOverride = SettingManager.GetSettingValue<bool>(SettingNames.PrimaryDomainControllerOverride);
                SettingManager.ChangeSetting(SettingNames.PrimaryDomainControllerOverride, (!pdcOverride).ToString());

                Output.WriteLine(!pdcOverride ? "Enabled" : "Disabled");
                ActionComplete<UsersPage>();
            });

        }
    }
}