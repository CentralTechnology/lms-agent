using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CRegeneratedTraceException : Exception
    {
        public CRegeneratedTraceException()
        {
        }

        protected CRegeneratedTraceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CRegeneratedTraceException(
            Exception innerException,
            string formatString,
            params object[] args)
            : base(string.Format(formatString, args), innerException)
        {
        }
    }
}
