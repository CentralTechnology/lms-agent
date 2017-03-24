namespace Service.Menu
{
    using System;
    using EasyConsole;
    using Pages;
    using Pages.Client;
    using Pages.Tools;
    using Pages.Tools.Account;
    using Pages.Tools.DebugPage;
    using Pages.Tools.Device;

    class ClientProgram : Program
    {
        public ClientProgram(Guid adminAccess)
            : base("License Monitoring System", true)
        {
            AddPage(new MainPage(this, adminAccess));
            AddPage(new AccountPage(this));
            AddPage(new DevicePage(this));
            AddPage(new DebugPage(this));
            AddPage(new ClientPage(this));
            AddPage(new ToolsPage(this));

            SetPage<MainPage>();
        }

        public override void Run()
        {
            Console.Title = Title;

            CurrentPage.Display();
        }
    }
}