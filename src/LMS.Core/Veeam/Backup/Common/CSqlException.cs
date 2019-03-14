using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CSqlException : CAppException
    {
        public CSqlException()
        {
        }

        protected CSqlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CSqlException(string formatString, params object[] arguments)
            : base(formatString, arguments)
        {
        }

        public CSqlException(Exception exc, string formatString, params object[] arguments)
            : base(exc, formatString, arguments)
        {
        }
    }
}
