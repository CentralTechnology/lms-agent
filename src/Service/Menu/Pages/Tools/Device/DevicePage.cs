using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Menu.Pages.Tools.Device
{
    using Abp.Dependency;
    using Account;
    using Core.Administration;
    using Core.Common.Extensions;
    using EasyConsole;

    public class DevicePage : MenuPage
    {
        public DevicePage(Program program)
            : base("Device", program)
        {
            using (var settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                Menu.Add("Update", () =>
                {
                    var newDeviceId = EasyConsoleExtensions.ReadGuid("Enter a new device id: ");

                    SettingsData settings = settingManager.Object.Read();
                    settingManager.Object.Update(new SettingsData
                    {
                        AccountId = settings.AccountId,
                        DeviceId = settings.DeviceId,
                        Monitors = settings.Monitors
                    });

                    Output.WriteLine(Environment.NewLine);
                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<DevicePage>();
                });
            }
        }

        public override void Display()
        {
            if (Program.History.Count > 1 && Program.BreadcrumbHeader)
            {
                string breadcrumb = null;
                foreach (string title in Program.History.Select(page => page.Title).Reverse())
                    breadcrumb += title + " > ";
                breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);
                Console.WriteLine(breadcrumb);
            }
            else
            {
                Console.WriteLine(Title);
            }
            Console.WriteLine("---");

            using (var settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                var deviceId = settingsManager.Object.Read().DeviceId;

                Output.WriteLine($"Device Id: {deviceId}");
                Output.WriteLine(Environment.NewLine);
            }

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.Display();
        }
    }
}
