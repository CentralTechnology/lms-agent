using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CCppCOMException : COMException, ICppComponentException
    {
        private readonly string _stackTrace;
        private readonly CCppTraceEventContainer _cppTraceEventContainer;
        private readonly int _errorCode;

        public CCppCOMException(
            string msg,
            string stackTrace,
            CCppTraceEventContainer traceEvents,
            int errorCode)
            : base(msg)
        {
            this._stackTrace = stackTrace;
            this._cppTraceEventContainer = traceEvents;
            this._errorCode = errorCode;
            this.Data.Add((object) nameof (CppTraceEvents), (object) this._cppTraceEventContainer);
        }

        protected CCppCOMException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
            this._stackTrace = (string) si.GetValue("stackTrace", typeof (string));
            this._cppTraceEventContainer = (CCppTraceEventContainer) si.GetValue("cppTraceEventContainer", typeof (CCppTraceEventContainer));
            this._errorCode = (int) si.GetValue("errorCode", typeof (int));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo si, StreamingContext context)
        {
            base.GetObjectData(si, context);
            si.AddValue("stackTrace", (object) this._stackTrace);
            si.AddValue("cppTraceEventContainer", (object) this._cppTraceEventContainer);
            si.AddValue("errorCode", this._errorCode);
        }

        public override int ErrorCode
        {
            get
            {
                return this._errorCode;
            }
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
