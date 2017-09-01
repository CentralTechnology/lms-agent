namespace Service.Menu.Pages.Run
{
    using System;
    using Abp.Threading;
    using Core.Administration;
    using Core.Common.Extensions;
    using Core.Users;
    using Core.Veeam;
    using EasyConsole;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;

    public class RunPage : MenuPage
    {
        private static readonly SettingManager SettingManager = new SettingManager();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected RavenClient RavenClient;

        public RunPage(Program program)
            : base("Run", program)
        {
            RavenClient = Core.Sentry.RavenClient.New();

            bool monitorUsers = SettingManager.GetSettingValue<bool>(SettingNames.MonitorUsers);
            if (monitorUsers)
            {
                Menu.Add(new Option("User Monitoring", () =>
                {
                    Logger.Info("User monitoring begin...");

                    try
                    {
                        AsyncHelper.RunSync(() => new UserOrchestrator().Start());

                        Logger.Info("************ User Monitoring Successful ************");
                    }
                    catch (Exception ex)
                    {
                        ex.Handle();
                        Logger.Error("************ User Monitoring Failed ************");
                    }
                    finally
                    {
                        Input.ReadString("Press [Enter]");
                        program.NavigateTo<RunPage>();
                    }

                    Input.ReadString("Press [Enter]");
                    program.NavigateTo<RunPage>();
                }));
            }

            bool monitorVeeam = SettingManager.GetSettingValue<bool>(SettingNames.MonitorVeeam);
            if (monitorVeeam)
            {
                Menu.Add(new Option("Veeam Monitoring", () =>
                {
                    Logger.Info("Veeam monitoring begin...");

                    try
                    {
                        AsyncHelper.RunSync(() => new VeeamOrchestrator().Start());

                        Logger.Info("************ Veeam Monitoring Successful ************");
                    }
                    catch (Exception ex)
                    {
                        ex.Handle();
                        Logger.Error("************ Veeam Monitoring Failed ************");
                    }
                    finally
                    {
                        Input.ReadString("Press [Enter]");
                        program.NavigateTo<RunPage>();
                    }
                }));
            }
        }
    }
}