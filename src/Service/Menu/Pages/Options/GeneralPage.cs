namespace LMS.Menu.Pages.Options
{
    using System;
    using Abp.Configuration;
    using Abp.Dependency;
    using Core.Configuration;
    using EasyConsole;

    class GeneralPage : MenuPage
    {
        public GeneralPage(Program program)
            : base("General", program)
        {
            var settingManager = IocManager.Instance.Resolve<ISettingManager>();

            Menu.Add("Set Account number", () =>
            {
                int newAcct = Input.ReadInt("Enter Account: ", int.MinValue, int.MaxValue);

                settingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, newAcct.ToString());

                IocManager.Instance.Release(settingManager);
                ActionComplete<GeneralPage>();
            });

            Menu.Add("Set Device number", () =>
            {
                Guid newDevice = Input.ReadGuid("Enter Device: ");

                settingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, newDevice.ToString());

                IocManager.Instance.Release(settingManager);
                ActionComplete<GeneralPage>();
            });
        }
    }
}