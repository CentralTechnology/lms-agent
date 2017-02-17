namespace LicenseMonitoringSystem.Menu
{
    using System;
    using EasyConsole;
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
            AddPage(new ExecutePage(this));
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