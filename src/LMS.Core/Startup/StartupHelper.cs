namespace LMS.Core.StartUp
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Abp.Configuration;
    using Abp.Events.Bus;
    using Abp.Logging;
    using Abp.Timing;
    using Configuration;
    using global::Hangfire;
    using global::Hangfire.Common;
    using Hangfire;
    using Helpers;
    using Users;
    using Users.Events;
    using Veeam;

    public class StartupHelper
    {
        private const string DefaultSchedule = "{0} */2 * * *";

        private static void ConfigureActiveDirectoryListener()
        {
            try
            {
                var eventLog = new EventLog("security");
                eventLog.EntryWritten += ListenForNewUsers;
                eventLog.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error(ex.Message, ex);
                throw;
            }

            LogHelper.Logger.Info("Monitoring the system for new users.");
        }

        public static void ConfigureBackgroundJobs(ISettingManager settingManager)
        {
            var recurringJobManager = new RecurringJobManager();

            recurringJobManager.RemoveIfExists(BackgroundJobNames.Users);
            recurringJobManager.RemoveIfExists(BackgroundJobNames.Veeam);

            ConfigureUsers(recurringJobManager, settingManager);
            ConfigureVeeam(recurringJobManager, settingManager);
        }

        private static void ConfigureUsers(IRecurringJobManager recurringJobManager, ISettingManager settingManager)
        {
            if (settingManager.GetSettingValue<bool>(AppSettingNames.MonitorUsers))
            {
                // setup event log monitoring
                ConfigureActiveDirectoryListener();

                string runtime = GetRunTime();

                recurringJobManager.AddOrUpdate(BackgroundJobNames.Users, Job.FromExpression<UserWorkerManager>(j => j.StartAsync(null)), runtime);
            }
        }

        private static void ConfigureVeeam(IRecurringJobManager recurringJobManager, ISettingManager settingManager)
        {
            if (settingManager.GetSettingValue<bool>(AppSettingNames.MonitorVeeam))
            {
                string runtime = GetRunTime();

                recurringJobManager.AddOrUpdate(BackgroundJobNames.Veeam, Job.FromExpression<VeeamWorkerManager>(j => j.StartAsync(null)), runtime);
            }
        }

        private static string GetRunTime()
        {
            return string.Format(DefaultSchedule, Clock.Now.Minute);
        }

        public static void Initiliaze(IStartupManager startupManager)
        {
            startupManager.Init();
        }

        private static void ListenForNewUsers(object source, EntryWrittenEventArgs e)
        {
            if ((ushort) e.Entry.InstanceId != 4720)
            {
                return;
            }

            LogHelper.Logger.Info($"New Event - Instance {e.Entry.InstanceId}  Type {e.Entry.EntryType}  Source {e.Entry.Source}");
            EventLogEntry entry = e.Entry;

            // sometimes it reschedules the jobs straight away.
            // make sure they are cancelled!
            LogHelper.Logger.Info("Event triggered - Cancelling any running jobs");
            HangfireHelper.CancelAllJobs();
            Thread.Sleep(10000);
            HangfireHelper.CancelAllJobs();

            EventBus.Default.Trigger(new NewActiveDirectoryUserEventData(entry));
        }
    }
}