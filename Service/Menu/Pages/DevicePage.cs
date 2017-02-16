namespace LicenseMonitoringSystem.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Core.Common.Extensions;
    using Core.Settings;
    using EasyConsole;

    class DevicePage : Page
    {
        private readonly ISettingManager _settingManager;
        public DevicePage(Program program)
            : base("Device", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();

            Options = new List<Option>
            {
                new Option("Set", () =>
                {
                    Guid deviceId = EasyConsoleExtensions.ReadGuid("Enter a new device id: ");

                    _settingManager.SetDeviceId(deviceId);

                    Output.WriteLine($"Device id is currently set to {deviceId}");

                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<DevicePage>();
                }),
                new Option("Home", program.NavigateHome)
            };
        }

        private IList<Option> Options { get; }

        public override void Display()
        {
            base.Display();

            var deviceId = _settingManager.GetDeviceId() ?? _settingManager.GetDeviceId(true);

            Output.WriteLine($"Device id is currently set to {deviceId}");

            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);
            }

            int choice = Input.ReadInt("Choose and option:", 1, Options.Count);

            Options[choice - 1].Callback();
        }
    }
}