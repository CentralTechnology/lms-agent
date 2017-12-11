namespace LMS.Service
{
    using System.Diagnostics;
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

            LogHelper.Logger.Debug($"New Event - Instance {e.Entry.InstanceId}  Type {e.Entry.EntryType}  Source {e.Entry.Source}");
            EventLogEntry entry = e.Entry;

            LogHelper.Logger.Info("Event triggered - Cancelling any running jobs");
            HangfireHelper.CancelAllJobs();

            EventBus.Default.Trigger(new NewActiveDirectoryUserEventData(entry));
        }
    }
}