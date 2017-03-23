namespace Service.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Core.Administration;
    using Core.Common.Extensions;
    using EasyConsole;

    class DevicePage : Page
    {
        private readonly ISettingsManager _settingManager;

        public DevicePage(Program program)
            : base("Device", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingsManager>();

            Options = new List<Option>
            {
                new Option("Set", () =>
                {
                    Guid deviceId = EasyConsoleExtensions.ReadGuid("Enter a new device id: ");

                    SettingsData settings = _settingManager.Read();
                    settings.DeviceId = deviceId;
                    _settingManager.Update(settings);

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

            Guid deviceId = _settingManager.Read().DeviceId;

            Output.WriteLine($"Device id is currently set to {deviceId}");

            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);
            }

            int choice = Input.ReadInt("Option:", 1, Options.Count);
            Output.WriteLine(ConsoleColor.White, $"{Environment.NewLine}Output");
            Options[choice - 1].Callback();
        }
    }
}