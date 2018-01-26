namespace LMS.Service
{
    using System.Diagnostics;
    using System.Threading;
    using Abp.Events.Bus;
    using Abp.Logging;
    using Common.Helpers;
    using Users.Events;

    public static class ServiceHost
    {
        public static void ConfigureEventLog()
        {
            var eventLog = new EventLog("security");
            eventLog.EntryWritten += ListenForNewUsers;
            eventLog.EnableRaisingEvents = true;

            LogHelper.Logger.Info("Monitoring the system for new users.");
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