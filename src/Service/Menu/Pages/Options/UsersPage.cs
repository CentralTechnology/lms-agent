namespace LMS.Menu.Pages.Options
{
    using Abp.Configuration;
    using Abp.Dependency;
    using Core.Configuration;
    using EasyConsole;

    class UsersPage : MenuPage
    {
        private readonly ISettingManager _settingManager;

        public UsersPage(Program program)
            : base("Users", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            
            Menu.Add("Manual Override", () =>
            {
                var enabled = _settingManager.GetSettingValue<bool>(AppSettingNames.UsersOverride);
                _settingManager.ChangeSettingForApplication(AppSettingNames.UsersOverride, (!enabled).ToString());

                Output.WriteLine(!enabled ? "Enabled" : "Disabled");
                IocManager.Instance.Release(_settingManager);
                ActionComplete<UsersPage>();
            });

            Menu.Add("PDC Override: ", () =>
            {
                bool pdcOverride = _settingManager.GetSettingValue<bool>(AppSettingNames.PrimaryDomainControllerOverride);
                _settingManager.ChangeSettingForApplication(AppSettingNames.PrimaryDomainControllerOverride, (!pdcOverride).ToString());

                Output.WriteLine(!pdcOverride ? "Enabled" : "Disabled");
                IocManager.Instance.Release(_settingManager);
                ActionComplete<UsersPage>();
            });

        }
    }
}