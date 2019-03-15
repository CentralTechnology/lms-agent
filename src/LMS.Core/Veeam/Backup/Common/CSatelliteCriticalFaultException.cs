using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CSatelliteCriticalFaultException : Exception
    {
        public CSatelliteCriticalFaultException()
        {
        }

        protected CSatelliteCriticalFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CSatelliteCriticalFaultException(
            Exception innerException,
            string formatString,
            params object[] arguments)
            : base(string.Format(formatString, arguments), innerException)
        {
        }

        public CSatelliteCriticalFaultException(string formatString, params object[] arguments)
            : this((Exception) null, formatString, arguments)
        {
        }
    }
}
