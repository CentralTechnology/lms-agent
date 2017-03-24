namespace Service.Menu.Pages
{
    using System;
    using Client;
    using Core.Common;
    using EasyConsole;
    using Tools;

    class MainPage : MenuPage
    {
        public MainPage(Program program, Guid adminAccess)
            : base("Main Page", program)
        {
            if (adminAccess == Guid.Parse(LmsConstants.AdminAccess))
            {
                Menu.Add(new Option("Admin", () => Console.WriteLine("Admin")));
            }

            Menu.Add(new Option("Client", () => program.NavigateTo<ClientPage>()));
            Menu.Add(new Option("Tools", () => program.NavigateTo<ToolsPage>()));
            Menu.Add(new Option("Exit", () => Environment.Exit(0)));
        }
    }
}