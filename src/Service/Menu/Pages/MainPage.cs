namespace LMS.Service.Menu.Pages
{
    using System;
    using Core.Common.Constants;
    using EasyConsole;
    using Options;
    using Run;

    class MainPage : MenuPage
    {
        public MainPage(Program program, Guid adminAccess)
            : base("Main Page", program)
        {
            if (adminAccess == Guid.Parse(Constants.AdminAccess))
            {
                Menu.Add(new Option("Admin", () => Console.WriteLine(@"Admin")));
            }

            Menu.Add(new Option("Run", () => program.NavigateTo<RunPage>()));
            Menu.Add(new Option("Options", () => program.NavigateTo<OptionsPage>()));
            Menu.Add(new Option("Exit", () => Environment.Exit(0)));
        }
    }
}