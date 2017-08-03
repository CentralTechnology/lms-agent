namespace Service.Menu.Pages.Options
{
    using System;
    using Core.Administration;
    using Core.Common.Extensions;
    using EasyConsole;

    class GeneralPage : MenuPage
    {
        private static readonly SettingManager SettingManager = new SettingManager();

        public GeneralPage(Program program)
            : base("General", program)
        {
            Menu.Add("Set Account number", () =>
            {
                int newAcct = Input.ReadInt("Enter Account: ", int.MinValue, int.MaxValue);

                SettingManager.ChangeSetting(SettingNames.AutotaskAccountId, newAcct.ToString());

                ActionComplete<GeneralPage>();
            });

            Menu.Add("Set Device number", () =>
            {
                Guid newDevice = Input.ReadGuid("Enter Device: ");

                SettingManager.ChangeSetting(SettingNames.CentrastageDeviceId, newDevice.ToString());

                ActionComplete<GeneralPage>();
            });
        }
    }
}