using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CVbrServiceMaintainceException : Exception
    {
        public CVbrServiceMaintainceException()
        {
        }

        protected CVbrServiceMaintainceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CVbrServiceMaintainceException(
            Exception innerException,
            string formatString,
            params object[] arguments)
            : base(string.Format(formatString, arguments), innerException)
        {
        }

        public CVbrServiceMaintainceException(string formatString, params object[] arguments)
            : this((Exception) null, formatString, arguments)
        {
        }
    }
}
