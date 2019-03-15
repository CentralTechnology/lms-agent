using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CAppException : ApplicationException
    {
        public CAppException()
        {
        }

        protected CAppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CAppException(string formatString, params object[] arguments)
            : base(string.Format(formatString, arguments))
        {
        }

        public CAppException(Exception exc, string formatString, params object[] arguments)
            : base(string.Format(formatString, arguments), exc)
        {
        }
    }
}
