using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Menu.Pages
{
    using Abp.Dependency;
    using Core.Settings;
    using EasyConsole;

    public class CachePage : Page
    {
        public CachePage(Program program)
            : base("Cache", program)
        {
            ISettingManager settingManager = IocManager.Instance.Resolve<ISettingManager>();

            Options = new List<Option>
            {
                new Option("Clear", () =>
                {
                    settingManager.ClearCache();

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

            int choice = Input.ReadInt("Choose and option:", 1, Options.Count);

            Options[choice - 1].Callback();
        }
    }
}
