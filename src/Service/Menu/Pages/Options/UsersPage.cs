namespace Service.Menu.Pages.Options
{
    using Core.Administration;
    using Core.Common.Extensions;
    using EasyConsole;

    class UsersPage : MenuPage
    {
        private static readonly SettingManager SettingManager = new SettingManager();

        public UsersPage(Program program)
            : base("Users", program)
        {
            

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