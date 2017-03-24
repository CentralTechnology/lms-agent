using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Menu.Pages.Client
{
    using Abp.Dependency;
    using Core;
    using Core.Administration;
    using Core.Common.Enum;
    using Core.Common.Extensions;
    using EasyConsole;

    class ClientPage : MenuPage
    {
        /// <inheritdoc />
        public ClientPage(Program program) 
            : base("Client Actions", program)
        {
            using (var settingsManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                List<Monitor> monitors = settingsManager.Object.Read().Monitors.GetFlags().OrderBy(m => m).ToList();

                foreach (var monitor in monitors)
                {
                    using (var orchestrator = IocManager.Instance.ResolveAsDisposable< IOrchestratorManager>())
                    {
                        Menu.Add(new Option(monitor.ToString(), () =>
                        {
                            // clear the screen
                            Console.Clear();


                            // run the action
                            orchestrator.Object.Run(monitor);

                            // reload the window
                            Input.ReadString("Press [Enter]");
                            program.NavigateTo<ClientPage>();
                        }));
                    }                    
                }               
            }
        }
    }
}
