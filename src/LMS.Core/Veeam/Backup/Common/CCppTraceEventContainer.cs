using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CCppTraceEventContainer
    {
        private List<CTraceEventInfo> _events = new List<CTraceEventInfo>();
        private List<CTraceProperty> _properties = new List<CTraceProperty>();
        public const string CppTraceEvtsDataKey = "CppTraceEvents";

        public void Add(CTraceEventInfo traceEvt)
        {
            this._events.Add(traceEvt);
        }

        public void Add(CTraceProperty traceProp)
        {
            this._properties.Add(traceProp);
        }

        public bool Has(ETraceEvent eventId)
        {
            return this._events.Any<CTraceEventInfo>((Func<CTraceEventInfo, bool>) (traceEvt => traceEvt.EventId == eventId));
        }

        public bool HasProperty(string key)
        {
            return this._properties.Any<CTraceProperty>((Func<CTraceProperty, bool>) (prop => string.Compare(prop.Key, key) == 0));
        }

        public string FindPropertyValue(string key)
        {
            return this._properties.Where<CTraceProperty>((Func<CTraceProperty, bool>) (p => string.Compare(p.Key, key) == 0)).FirstOrDefault<CTraceProperty>()?.Value;
        }
    }
}
