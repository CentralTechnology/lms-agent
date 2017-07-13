namespace Service.Menu.Pages.Tools.Account
{
    using System;
    using Abp.Dependency;
    using Core.Administration;
    using Core.Factory;
    using EasyConsole;

    public class AccountPage : MenuPage
    {
        public AccountPage(Program program)
            : base("Account", program)
        {
                Menu.Add("Update", () =>
                {
                    int newAcctId = Input.ReadInt("New Account Id: ", int.MinValue, int.MaxValue);

                    SettingsData settings = SettingFactory.SettingsManager().Read();
                    SettingFactory.SettingsManager().Update(new SettingsData
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

        public override void Display()
        {
            this.AddBreadCrumb();

                int acctId = SettingFactory.SettingsManager().Read().AccountId;

                Output.WriteLine($"Account Id: {acctId}");
            

            this.AddBackOption();

            Menu.Display();
        }
    }
}