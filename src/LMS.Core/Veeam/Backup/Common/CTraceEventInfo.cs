using System;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CTraceEventInfo
    {
        public readonly ETraceEvent EventId;
        public readonly string Description;

        public CTraceEventInfo(ETraceEvent eventId, string auxDescription)
        {
            this.EventId = eventId;
            this.Description = auxDescription;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Description))
                return string.Format("{0}, {1}", (object) this.EventId.ToString(), (object) this.Description.ToString());
            return this.EventId.ToString();
        }
    }
}
