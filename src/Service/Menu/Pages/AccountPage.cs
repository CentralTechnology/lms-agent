namespace Service.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Core.Administration;
    using EasyConsole;

    class AccountPage : Page
    {
        private readonly ISettingsManager _settingManager;

        public AccountPage(Program program)
            : base("Account", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingsManager>();

            Options = new List<Option>
            {
                new Option("Set", () =>
                {
                    int acctId = Input.ReadInt("Enter a new account id: ", int.MinValue, int.MaxValue);

                    SettingsData settings = _settingManager.Read();
                    settings.AccountId = acctId;
                    _settingManager.Update(settings);

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

            int acctId = _settingManager.Read().AccountId;
            Output.WriteLine($"Account id is currently set to {acctId}");

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