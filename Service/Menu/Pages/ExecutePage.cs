using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Menu.Pages
{
    using Abp.Dependency;
    using Core;
    using Core.Settings;
    using EasyConsole;

    class ExecutePage : Page
    {
        private readonly ISettingManager _settingManager;
        private readonly Orchestrator _orchestrator;
        public ExecutePage(Program program) 
            : base("Execute", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _orchestrator = IocManager.Instance.Resolve<Orchestrator>();

            var monitors = _settingManager.GetMonitors();

            Options = new List<Option>();

            foreach (var monitor in monitors)
            {
                Options.Add(new Option(monitor.ToString(), () =>
                {
                    _orchestrator.Run(monitor);

                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<ExecutePage>();
                }));
            }

            Options.Add(new Option("Home", program.NavigateHome));
        }

        private IList<Option> Options { get; set; }

        public override void Display()
        {
            base.Display();

            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);
            }

            int choice = Input.ReadInt("Choose and option:", 1, Options.Count);

            Options[choice - 1].Callback();
        }
    }
}
