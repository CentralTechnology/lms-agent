using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Menu
{
    using EasyConsole;
    using Pages;

    class ClientProgram : Program
    {
        public ClientProgram()
            : base("Console", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));
            AddPage(new AccountPage(this));
            AddPage(new CachePage(this));
            AddPage(new DevicePage(this));

            SetPage<MainPage>();
        }
    }
}
