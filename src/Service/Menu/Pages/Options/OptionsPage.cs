namespace LMS.Menu.Pages.Options
{
    using EasyConsole;

    public class OptionsPage : MenuPage
    {
        public OptionsPage(Program program)
            : base("Options", program)
        {
            Menu.Add(new Option("General", () => program.NavigateTo<GeneralPage>()));
            Menu.Add(new Option("Users", () => program.NavigateTo<UsersPage>()));
            Menu.Add(new Option("Veeam", () => program.NavigateTo<VeeamPage>()));
        }
    }
}