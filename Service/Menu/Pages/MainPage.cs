namespace LicenseMonitoringSystem.Menu.Pages
{
    using System;
    using EasyConsole;

    class MainPage : MenuPage
    {
        public MainPage(Program program)
            : base("Main Page", program,
                new Option("Account", () => program.NavigateTo<AccountPage>()),
                new Option("Cache", () => program.NavigateTo<CachePage>()),
                new Option("Device", () => program.NavigateTo<DevicePage>()),
                new Option("Debug", () => program.NavigateTo<DebugPage>()),
                new Option("Actions", () => program.NavigateTo<ActionsPage>()),
                new Option("Exit", () => Environment.Exit(0))
            )
        {
        }
    }
}