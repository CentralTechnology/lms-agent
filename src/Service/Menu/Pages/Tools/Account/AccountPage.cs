namespace Service.Menu.Pages.Tools.Account
{
    using System;
    using Core.Administration;
    using Core.Common.Extensions;
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

                SettingFactory.SettingsManager().ChangeSetting(SettingNames.AutotaskAccountId, newAcctId.ToString());

                Output.WriteLine(Environment.NewLine);
                Input.ReadString("Press [Enter]");
                Program.NavigateTo<AccountPage>();
            });
        }

        public override void Display()
        {
            this.AddBreadCrumb();

            int acctId = SettingFactory.SettingsManager().GetSettingValue<int>(SettingNames.AutotaskAccountId);

            Output.WriteLine($"Account Id: {acctId}");

            this.AddBackOption();

            Menu.Display();
        }
    }
}