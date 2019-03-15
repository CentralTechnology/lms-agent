using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CCppComponentException : CAppException, ICppComponentException
    {
        private readonly string _stackTrace;
        private readonly CCppTraceEventContainer _cppTraceEventContainer;

        public CCppComponentException(
            string msg,
            string stackTrace,
            CCppTraceEventContainer traceEvents)
            : base(msg)
        {
            this._stackTrace = stackTrace;
            this._cppTraceEventContainer = traceEvents;
            this.Data.Add((object) nameof (CppTraceEvents), (object) this._cppTraceEventContainer);
        }

        protected CCppComponentException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
            this._stackTrace = (string) si.GetValue("stackTrace", typeof (string));
            this._cppTraceEventContainer = (CCppTraceEventContainer) si.GetValue("cppTraceEventContainer", typeof (CCppTraceEventContainer));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo si, StreamingContext context)
        {
            base.GetObjectData(si, context);
            si.AddValue("stackTrace", (object) this._stackTrace);
            si.AddValue("cppTraceEventContainer", (object) this._cppTraceEventContainer);
        }

        public CCppTraceEventContainer CppTraceEvents
        {
            get
            {
                return this._cppTraceEventContainer;
            }
        }

        public override string StackTrace
        {
            get
            {
                return this._stackTrace;
            }
        }
    }
}
