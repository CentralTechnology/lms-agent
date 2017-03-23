namespace Service.Menu.Pages
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Core.Administration;
    using EasyConsole;

    public class CachePage : Page
    {
        public CachePage(Program program)
            : base("Cache", program)
        {
            ISettingsManager settingManager = IocManager.Instance.Resolve<ISettingsManager>();

            Options = new List<Option>
            {
                new Option("Clear", () =>
                {
                    //settingManager.ClearCache();

                    Output.WriteLine(ConsoleColor.Green, "Cache successfully cleared!");
                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<CachePage>();
                }),
                new Option("Home", program.NavigateHome)
            };
        }

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