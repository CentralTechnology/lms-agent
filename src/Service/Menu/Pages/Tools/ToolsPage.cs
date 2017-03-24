namespace Service.Menu.Pages.Tools
{
    using Account;
    using Debug;
    using Device;
    using EasyConsole;

    public class ToolsPage : MenuPage
    {
        public ToolsPage(Program program)
            : base("Tools", program)
        {
            Menu.Add("Account", () => program.NavigateTo<AccountPage>());
            Menu.Add("Device", () => program.NavigateTo<DevicePage>());
            Menu.Add("Debug", () => program.NavigateTo<DebugPage>());
        }
    }
}