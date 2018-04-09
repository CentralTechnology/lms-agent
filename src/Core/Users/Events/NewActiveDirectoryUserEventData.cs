namespace LMS.Core.Users.Events
{
    using System.Diagnostics;
    using Abp.Events.Bus;

    public class NewActiveDirectoryUserEventData : EventData
    {
        public NewActiveDirectoryUserEventData()
        {
        }

        public NewActiveDirectoryUserEventData(EventLogEntry entry)
        {
            Entry = entry;
        }

        public EventLogEntry Entry { get; set; }
    }
}