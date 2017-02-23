namespace LicenseMonitoringSystem.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core;
    using Core.Settings;
    using EasyConsole;

    class ActionsPage : Page
    {
        public ActionsPage(Program program)
            : base("Actions", program)
        {
            Logger = IocManager.Instance.Resolve<ILogger>();
            ISettingManager settingManager = IocManager.Instance.Resolve<ISettingManager>();
            Orchestrator orchestrator = IocManager.Instance.Resolve<Orchestrator>();

            var monitors = settingManager.GetMonitors();

            Options = new List<Option>();

            foreach (var monitor in monitors)
            {
                Options.Add(new Option(monitor.ToString(), () =>
                {
                    try
                    {
                        orchestrator.Run(monitor);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }

                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<ActionsPage>();
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

            int choice = Input.ReadInt("Option:", 1, Options.Count);
            Output.WriteLine(ConsoleColor.White, $"{Environment.NewLine}Output");
            Options[choice - 1].Callback();
        }
    }
}