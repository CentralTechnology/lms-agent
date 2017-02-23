namespace LicenseMonitoringSystem.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Core.Settings;
    using EasyConsole;

    class AccountPage : Page
    {
        private readonly ISettingManager _settingManager;

        public AccountPage(Program program)
            : base("Account", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();

            Options = new List<Option>
            {
                new Option("Set", () =>
                {
                    int acctId = Input.ReadInt("Enter a new account id: ", int.MinValue, int.MaxValue);

                    _settingManager.SetAccountId(acctId);

                    Output.WriteLine($"Account id has been updated to {acctId}");

                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<AccountPage>();
                }),
                new Option("Home", program.NavigateHome)
            };
        }

        private IList<Option> Options { get; }

        public override void Display()
        {
            base.Display();

            var acctId = _settingManager.GetAccountId();
            if (acctId == null)
            {
                Output.WriteLine(ConsoleColor.Red, "Account id is not currently set");
            }
            else
            {
                Output.WriteLine($"Account id is currently set to {acctId}");
            }

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