namespace Service.Menu.Pages.Tools.Device
{
    using System;
    using System.Linq;
    using Abp.Dependency;
    using Common;
    using Core.Administration;
    using EasyConsole;

    public class DevicePage : MenuPage
    {
        public DevicePage(Program program)
            : base("Device", program)
        {
            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                Menu.Add("Update", () =>
                {
                    Guid newDeviceId = EasyConsoleExtensions.ReadGuid("Enter a new device id: ");

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

            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                Guid deviceId = settingsManager.Object.Read().DeviceId;

                Output.WriteLine($"Device Id: {deviceId}");
            }

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.Display();
        }
    }
}