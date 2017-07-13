namespace Service.Menu.Pages.Tools.Debug
{
    using System;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core.Administration;
    using Core.Factory;
    using EasyConsole;

    public class DebugPage : MenuPage
    {
        public DebugPage(Program program)
            : base("Debug", program)
        {

            Menu.Add("Toggle", () =>
            {
                bool debug = SettingFactory.SettingsManager().ReadLoggerLevel() == LoggerLevel.Debug;
                SettingFactory.SettingsManager().UpdateLoggerLevel(!debug);

                Output.WriteLine(Environment.NewLine);
                Input.ReadString("Press [Enter]");
                Program.NavigateTo<DebugPage>();
            });
        }

        public override void Display()
        {
            this.AddBreadCrumb();

            LoggerLevel debug = SettingFactory.SettingsManager().ReadLoggerLevel();

            Output.WriteLine(debug == LoggerLevel.Debug ? "Status: enabled" : "Status: disabled");
            Output.WriteLine(Environment.NewLine);

            this.AddBackOption();

            Menu.Display();
        }
    }
}