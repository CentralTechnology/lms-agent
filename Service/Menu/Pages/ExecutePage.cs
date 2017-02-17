namespace LicenseMonitoringSystem.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core;
    using Core.Settings;
    using EasyConsole;

    class ExecutePage : Page
    {
        private readonly Orchestrator _orchestrator;
        private readonly ISettingManager _settingManager;

        public ExecutePage(Program program)
            : base("Execute", program)
        {
            Logger = IocManager.Instance.Resolve<ILogger>();
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _orchestrator = IocManager.Instance.Resolve<Orchestrator>();

            var monitors = _settingManager.GetMonitors();

            Options = new List<Option>();

            foreach (var monitor in monitors)
            {
                Options.Add(new Option(monitor.ToString(), () =>
                {
                    try
                    {
                        _orchestrator.Run(monitor);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }

                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<ExecutePage>();
                }));
            }

            Options.Add(new Option("Home", program.NavigateHome));
        }

        public ILogger Logger { get; set; }

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