namespace LMS.Menu.Pages.Run
{
    using System;
    using Abp.Configuration;
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Core.Common.Extensions;
    using Core.Configuration;
    using EasyConsole;
    using SharpRaven;
    using Users;
    using Veeam;

    public class RunPage : MenuPage, ITransientDependency
    {
        protected RavenClient RavenClient;
        private readonly ISettingManager _settingManager;
        private readonly UserOrchestrator _userOrchestrator;
        private readonly VeeamOrchestrator _veeamOrchestrator;

        public ILogger Logger { get; set; }

        public RunPage(Program program)
            : base("Run", program)
        {
            Logger = NullLogger.Instance;
            RavenClient = Core.Sentry.RavenClient.Instance;
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _userOrchestrator = IocManager.Instance.Resolve<UserOrchestrator>();
            _veeamOrchestrator = IocManager.Instance.Resolve<VeeamOrchestrator>();

            bool monitorUsers = _settingManager.GetSettingValue<bool>(AppSettingNames.MonitorUsers);
            if (monitorUsers)
            {
                Menu.Add(new Option("User Monitoring", () =>
                {
                    Console.Clear();
                    Logger.Info("User monitoring begin...");

                    try
                    {
                        _userOrchestrator.Start();

                        Logger.Info("************ User Monitoring Successful ************");
                    }
                    catch (Exception ex)
                    {
                        ex.Handle();
                        Logger.Error("************ User Monitoring Failed ************");
                    }
                    finally
                    {
                        IocManager.Instance.Release(_userOrchestrator);
                        Input.ReadString("Press [Enter]");
                        program.NavigateTo<RunPage>();
                    }

                    Input.ReadString("Press [Enter]");
                    program.NavigateTo<RunPage>();
                }));
            }

            bool monitorVeeam = _settingManager.GetSettingValue<bool>(AppSettingNames.MonitorVeeam);
            if (monitorVeeam)
            {
                Menu.Add(new Option("Veeam Monitoring", () =>
                {
                    Console.Clear();
                    Logger.Info("Veeam monitoring begin...");

                    try
                    {
                        _veeamOrchestrator.Start();

                        Logger.Info("************ Veeam Monitoring Successful ************");
                    }
                    catch (Exception ex)
                    {
                        ex.Handle();
                        Logger.Error("************ Veeam Monitoring Failed ************");
                    }
                    finally
                    {
                        IocManager.Instance.Release(_veeamOrchestrator);
                        Input.ReadString("Press [Enter]");
                        program.NavigateTo<RunPage>();
                    }
                }));
            }
        }
    }
}