using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Menu.Pages
{
    using EasyConsole;

    class MainPage : MenuPage
    {
        public MainPage(Program program) 
            : base("Main Page", program,
                  new Option("Account", () => program.NavigateTo<AccountPage>()),
                  new Option("Cache", () => program.NavigateTo<CachePage>()),
                  new Option("Device", () => program.NavigateTo<DevicePage>()),
                  new Option("Debug", () => program.NavigateTo<DebugPage>()),
                  new Option("Execute", () => program.NavigateTo<ExecutePage>()),
                  new Option("Exit", () => Environment.Exit(0))
                  )
        {
            
        }
    }
}
