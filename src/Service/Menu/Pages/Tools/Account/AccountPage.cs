namespace Service.Menu.Pages.Tools.Account
{
    using System;
    using System.Linq;
    using Abp.Dependency;
    using Core.Administration;
    using EasyConsole;

    public class AccountPage : MenuPage
    {
        public AccountPage(Program program)
            : base("Account", program)
        {
            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                Menu.Add("Update", () =>
                {
                    int newAcctId = Input.ReadInt("New Account Id: ", int.MinValue, int.MaxValue);

                    SettingsData settings = settingManager.Object.Read();
                    settingManager.Object.Update(new SettingsData
                    {
                        AccountId = newAcctId,
                        DeviceId = settings.DeviceId,
                        Monitors = settings.Monitors
                    });

                    Output.WriteLine(Environment.NewLine);
                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<AccountPage>();
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
                int acctId = settingsManager.Object.Read().AccountId;

                Output.WriteLine($"Account Id: {acctId}");
            }

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.Display();
        }
    }
}