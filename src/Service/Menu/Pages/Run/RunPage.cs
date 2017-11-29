namespace LMS.Menu.Pages.Run
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Security;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Logging;
    using Core.Configuration;
    using EasyConsole;
    using Microsoft.OData.Client;
    using SharpRaven;
    using SharpRaven.Data;
    using Users;
    using Veeam;

    public class RunPage : MenuPage
    {
        protected RavenClient RavenClient;

        public RunPage(Program program)
            : base("Run", program)
        {
            var settingManager = IocManager.Instance.Resolve<ISettingManager>();
            var userWorkerManager = IocManager.Instance.Resolve<IUserWorkerManager>();
            var veeamWorkerManager = IocManager.Instance.Resolve<IVeeamWorkerManager>();

            bool monitorUsers = settingManager.GetSettingValue<bool>(AppSettingNames.MonitorUsers);
            if (monitorUsers)
            {
                Menu.Add(new Option("User Monitoring", () =>
                {
                    Console.Clear();
                    LogHelper.Logger.Info("User monitoring begin...");

                    try
                    {
                        userWorkerManager.Start();

                        LogHelper.Logger.Info("************ User Monitoring Successful ************");
                    }
                    catch (Exception ex) when (
                        ex is DataServiceClientException
                        || ex is SqlException
                        || ex is HttpRequestException
                        || ex is SocketException
                        || ex is WebException
                        || ex is SecurityException
                        || ex is IOException)
                    {
                        LogHelper.LogException(ex);
                        LogHelper.Logger.Debug(ex.Message, ex);
                        LogHelper.Logger.Error("************ User Monitoring Failed ************");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogException(ex);
                        LogHelper.Logger.Debug(ex.Message, ex);
                        RavenClient.Capture(new SentryEvent(ex));
                        LogHelper.Logger.Error("************ User Monitoring Failed ************");
                    }
                    finally
                    {
                        IocManager.Instance.Release(userWorkerManager);
                        Input.ReadString("Press [Enter]");
                        program.NavigateTo<RunPage>();
                    }

                    Input.ReadString("Press [Enter]");
                    program.NavigateTo<RunPage>();
                }));
            }

            bool monitorVeeam = settingManager.GetSettingValue<bool>(AppSettingNames.MonitorVeeam);
            if (monitorVeeam)
            {
                Menu.Add(new Option("Veeam Monitoring", () =>
                {
                    Console.Clear();
                    LogHelper.Logger.Info("Veeam monitoring begin...");

                    try
                    {
                        veeamWorkerManager.Start();

                        LogHelper.Logger.Info("************ Veeam Monitoring Successful ************");
                    }
                    catch (Exception ex) when (
                        ex is DataServiceClientException
                        || ex is SqlException
                        || ex is HttpRequestException
                        || ex is SocketException
                        || ex is WebException
                        || ex is SecurityException
                        || ex is IOException)
                    {
                        LogHelper.LogException(ex);
                        LogHelper.Logger.Debug(ex.Message, ex);
                        LogHelper.Logger.Error("************ Veeam Monitoring Failed ************");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogException(ex);
                        LogHelper.Logger.Debug(ex.Message, ex);
                        RavenClient.Capture(new SentryEvent(ex));
                        LogHelper.Logger.Error("************ Veeam Monitoring Failed ************");
                    }
                    finally
                    {
                        IocManager.Instance.Release(veeamWorkerManager);
                        Input.ReadString("Press [Enter]");
                        program.NavigateTo<RunPage>();
                    }
                }));
            }
        }
    }
}