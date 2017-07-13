namespace Service.Menu.Pages.Tools.Device
{
    using System;
    using Abp.Dependency;
    using Common;
    using Core.Administration;
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

                    SettingsData settings = SettingFactory.SettingsManager().Read();
                    SettingFactory.SettingsManager().Update(new SettingsData
                    {
                        AccountId = settings.AccountId,
                        DeviceId = newDeviceId,
                        Monitors = settings.Monitors
                    });

                    Output.WriteLine(Environment.NewLine);
                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<DevicePage>();
                });
            
        }

        public override void Display()
        {
            this.AddBreadCrumb();

                Guid deviceId = SettingFactory.SettingsManager().Read().DeviceId;

                Output.WriteLine($"Device Id: {deviceId}");
            

            this.AddBackOption();

            Menu.Display();
        }
    }
}