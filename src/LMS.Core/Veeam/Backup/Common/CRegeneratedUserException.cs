using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CRegeneratedUserException : Exception
    {
        public CRegeneratedUserException()
        {
        }

        protected CRegeneratedUserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CRegeneratedUserException(
            Exception innerException,
            string formatString,
            params object[] arguments)
            : base(string.Format(formatString, arguments), innerException)
        {
        }
    }
}
