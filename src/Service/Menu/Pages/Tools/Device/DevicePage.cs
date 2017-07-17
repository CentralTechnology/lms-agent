namespace Service.Menu.Pages.Tools.Device
{
    using System;
    using Abp.Dependency;
    using Common;
    using Core.Administration;
    using Core.Common.Extensions;
    using Core.Factory;
    using EasyConsole;

    public class DevicePage : MenuPage
    {
        public DevicePage(Program program)
            : base("Device", program)
        {
            Menu.Add("Update", () =>
            {
                Guid newDeviceId = EasyConsoleExtensions.ReadGuid("Enter a new device id: ");

                SettingFactory.SettingsManager().ChangeSetting(SettingNames.CentrastageDeviceId, newDeviceId.ToString());

                Output.WriteLine(Environment.NewLine);
                Input.ReadString("Press [Enter]");
                Program.NavigateTo<DevicePage>();
            });

        }

        public override void Display()
        {
            this.AddBreadCrumb();

            Guid deviceId = SettingFactory.SettingsManager().GetSettingValue<Guid>(SettingNames.CentrastageDeviceId);

            Output.WriteLine($"Device Id: {deviceId}");

            this.AddBackOption();

            Menu.Display();
        }
    }
}