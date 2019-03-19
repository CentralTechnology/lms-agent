using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CMethodReconnectException : Exception
    {
        public CMethodReconnectException()
        {
        }

        protected CMethodReconnectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CMethodReconnectException(
            Exception innerException,
            string formatString,
            params object[] arguments)
            : base(string.Format(formatString, arguments), innerException)
        {
        }

        public CMethodReconnectException(string formatString, params object[] arguments)
            : this((Exception) null, formatString, arguments)
        {
        }
    }
}
