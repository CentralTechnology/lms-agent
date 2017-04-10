namespace Service.Menu.Pages.Tools.DebugPage
{
    using System;
    using System.Linq;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core.Administration;
    using EasyConsole;

    public class DebugPage : MenuPage
    {
        private readonly ISettingsManager _settingManager;

        public DebugPage(Program program)
            : base("Debug", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingsManager>();

            Menu.Add("Toggle", () =>
            {
                bool debug = _settingManager.ReadLoggerLevel() == LoggerLevel.Debug;
                _settingManager.UpdateLoggerLevel(!debug);

                Output.WriteLine(Environment.NewLine);
                Input.ReadString("Press [Enter]");
                Program.NavigateTo<DebugPage>();
            });
        }

        public override void Display()
        {
            if (Program.History.Count > 1 && Program.BreadcrumbHeader)
            {
                string breadcrumb = null;
                foreach (string title in Program.History.Select(page => page.Title).Reverse())
                    breadcrumb += title + " > ";
                breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);
                Console.WriteLine(breadcrumb);
            }
            else
            {
                Console.WriteLine(Title);
            }
            Console.WriteLine("---");

            LoggerLevel debug = _settingManager.ReadLoggerLevel();

            Output.WriteLine(debug == LoggerLevel.Debug ? "Status: enabled" : "Status: disabled");
            Output.WriteLine(Environment.NewLine);

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.Display();
        }
    }
}