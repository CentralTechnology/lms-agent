using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Events
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
