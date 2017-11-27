namespace LMS
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using Abp;
    using Abp.Configuration;
    using Abp.Dependency;
    using Castle.Facilities.Logging;
    using Common.Helpers;
    using Core.Configuration;
    using EntityFramework;
    using Menu;
    using ServiceTimer;
    using Startup;
    using Workers;

    public class LMSService : TimerServiceBase
    {
        private AbpBootstrapper _bootstrapper;

        public override bool Start()
        {
            try
            {
                _bootstrapper = AbpBootstrapper.Create<LMSServiceModule>();
                _bootstrapper.IocManager
                    .IocContainer
                    .AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

                _bootstrapper.Initialize();

                DefaultLog();

                Log.Info($"Version: {AppVersionHelper.Version}  Release: {AppVersionHelper.ReleaseDate}");

                using (IDisposableDependencyObjectWrapper<StartupManager> startupManager = _bootstrapper.IocManager.ResolveAsDisposable<StartupManager>())
                {
                    bool started = startupManager.Object.Init();
                    if (!started)
                    {
                        return false;
                    }

                    if (Environment.UserInteractive)
                    {
                        Console.Clear();
                        new ClientProgram(Guid.NewGuid()).Run();
                    }
                    else
                    {
                        using (IDisposableDependencyObjectWrapper<SettingManager> settingManager = _bootstrapper.IocManager.ResolveAsDisposable<SettingManager>())
                        {
                            if (settingManager.Object.GetSettingValue<bool>(AppSettingNames.MonitorVeeam))
                            {
                                var veeamMonitorWorker = new VeeamMonitorWorker();
                                RegisterWorker(veeamMonitorWorker);
                            }

                            if (settingManager.Object.GetSettingValue<bool>(AppSettingNames.MonitorUsers))
                            {
                                var userMonitorWorker = new UserMonitorWorker();
                                RegisterWorker(userMonitorWorker);
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("License Monitoring System",$"License Monitoring Ssytem service cannot be started. Exception message =  {ex.GetType().Name} : {ex.Message} | {ex.StackTrace}", EventLogEntryType.Error);
                return false;
            }
        }

        public override bool Stop()
        {
            _bootstrapper.Dispose();

            return base.Stop();
        }
    }
}