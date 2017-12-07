namespace LMS.Service.Menu.Pages.Options
{
    using Abp.Configuration;
    using Abp.Dependency;
    using Core.Configuration;
    using EasyConsole;

    class VeeamPage : MenuPage
    {
        public VeeamPage(Program program)
            : base("Veeam", program)
        {
            var settingManager = IocManager.Instance.Resolve<ISettingManager>();

            Menu.Add("Manual Override", () =>
            {
                var enabled = settingManager.GetSettingValue<bool>(AppSettingNames.VeeamOverride);
                settingManager.ChangeSettingForApplication(AppSettingNames.VeeamOverride, (!enabled).ToString());

                IocManager.Instance.Release(settingManager);
                Output.WriteLine(!enabled ? "Enabled" : "Disabled");
                ActionComplete<VeeamPage>();
            });
        }
    }
}