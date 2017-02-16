﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Menu.Pages
{
    using Abp.Dependency;
    using Core.Settings;
    using EasyConsole;

    class DebugPage : Page
    {
        private readonly ISettingManager _settingManager;
        public DebugPage(Program program)
            : base("Debug", program)
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();

            Options = new List<Option>
            {
                new Option("Toggle", () =>
                {
                    _settingManager.SetDebug(!_settingManager.Debug);

                    Input.ReadString("Press [Enter]");
                    Program.NavigateTo<DebugPage>();
                }),
                new Option("Home", program.NavigateHome)
            };
        }

        private IList<Option> Options { get; set; }

        public override void Display()
        {
            base.Display();

            var debug = _settingManager.Debug;

            Console.WriteLine(debug ? "Debugging is currently enabled." : "Debugging is currently disabled.");

            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);
            }

            int choice = Input.ReadInt("Choose and option:", 1, Options.Count);

            Options[choice - 1].Callback();
        }
    }
}
