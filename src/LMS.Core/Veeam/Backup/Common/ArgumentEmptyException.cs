using System;
using System.Runtime.Serialization;
using System.Security;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class ArgumentEmptyException : ArgumentException
    {
        public ArgumentEmptyException()
        {
        }

        public ArgumentEmptyException(string paramName)
            : base(paramName)
        {
        }

        public ArgumentEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ArgumentEmptyException(string paramName, string message)
            : base(message, paramName)
        {
        }

        [SecurityCritical]
        protected ArgumentEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
