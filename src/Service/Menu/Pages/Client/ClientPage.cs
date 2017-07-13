namespace Service.Menu.Pages.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Dependency;
    using Core;
    using Core.Administration;
    using Core.Common.Enum;
    using Core.Common.Extensions;
    using Core.Factory;
    using EasyConsole;

    class ClientPage : MenuPage
    {
        /// <inheritdoc />
        public ClientPage(Program program)
            : base("Client Actions", program)
        {
                List<Monitor> monitors = SettingFactory.SettingsManager().Read().Monitors.GetFlags().OrderBy(m => m).ToList();

                foreach (Monitor monitor in monitors)
                {
                        Menu.Add(new Option(monitor.ToString(), () =>
                        {
                            // clear the screen
                            Console.Clear();

                            // run the action
                            OrchestratorFactory.Orchestrator().Run(monitor);

                            // reload the window
                            Input.ReadString("Press [Enter]");
                            program.NavigateTo<ClientPage>();
                        }));
                    
                
            }
        }
    }
}