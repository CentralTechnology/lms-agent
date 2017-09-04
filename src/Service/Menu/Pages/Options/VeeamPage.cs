namespace Service.Menu.Pages.Options
{
    using Core.Administration;
    using Core.Common.Extensions;
    using EasyConsole;

    class VeeamPage : MenuPage
    {
        private static readonly SettingManager SettingManager = new SettingManager();

        public VeeamPage(Program program)
            : base("Veeam", program)
        {
            Menu.Add("Manual Override", () =>
            {
                var enabled = SettingManager.GetSettingValue<bool>(SettingNames.VeeamOverride);
                SettingManager.ChangeSetting(SettingNames.VeeamOverride, (!enabled).ToString());

                Output.WriteLine(!enabled ? "Enabled" : "Disabled");
                ActionComplete<VeeamPage>();
            });
        }
    }
}