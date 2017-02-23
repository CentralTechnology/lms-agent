namespace Service.Menu
{
    using System;
    using EasyConsole;
    using LicenseMonitoringSystem.Menu.Pages;
    using Pages;

    class ClientProgram : Program
    {
        public ClientProgram()
            : base("License Monitoring System", true)
        {
            AddPage(new MainPage(this));
            AddPage(new AccountPage(this));
            AddPage(new CachePage(this));
            AddPage(new DevicePage(this));
            AddPage(new ActionsPage(this));
            AddPage(new DebugPage(this));

            SetPage<MainPage>();
        }

        public override void Run()
        {
            Console.Title = Title;

            CurrentPage.Display();
        }
    }
}