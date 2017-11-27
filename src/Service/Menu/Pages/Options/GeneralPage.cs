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
            Menu.Add("Set Account number", () =>
            {
                int newAcct = Input.ReadInt("Enter Account: ", int.MinValue, int.MaxValue);

                using (IDisposableDependencyObjectWrapper<ISettingManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
                {
                    settingManager.Object.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, newAcct.ToString());
                }

                ActionComplete<GeneralPage>();
            });

            Menu.Add("Set Device number", () =>
            {
                Guid newDevice = Input.ReadGuid("Enter Device: ");

                using (IDisposableDependencyObjectWrapper<ISettingManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
                {
                    settingManager.Object.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, newDevice.ToString());
                }

                ActionComplete<GeneralPage>();
            });
        }
    }
}