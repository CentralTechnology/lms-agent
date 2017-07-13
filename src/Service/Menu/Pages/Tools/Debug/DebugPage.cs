namespace Service.Menu.Pages.Tools.Debug
{
    using System;
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
            this.AddBreadCrumb();

            LoggerLevel debug = _settingManager.ReadLoggerLevel();

            Output.WriteLine(debug == LoggerLevel.Debug ? "Status: enabled" : "Status: disabled");
            Output.WriteLine(Environment.NewLine);

            this.AddBackOption();

            Menu.Display();
        }
    }
}