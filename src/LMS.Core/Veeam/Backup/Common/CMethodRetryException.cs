using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CMethodRetryException : Exception
    {
        public CMethodRetryException()
        {
        }

        protected CMethodRetryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CMethodRetryException(
            Exception innerException,
            string formatString,
            params object[] arguments)
            : base(string.Format(formatString, arguments), innerException)
        {
        }

        public CMethodRetryException(string formatString, params object[] arguments)
            : this((Exception) null, formatString, arguments)
        {
        }
    }
}
