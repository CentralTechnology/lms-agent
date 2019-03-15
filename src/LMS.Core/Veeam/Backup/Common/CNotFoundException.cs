using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CNotFoundException : Exception
    {
        public CNotFoundException()
        {
        }

        protected CNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CNotFoundException(string formatString, params object[] arguments)
            : base(string.Format(formatString, arguments))
        {
        }

        public CNotFoundException(
            Exception innerException,
            string formatString,
            params object[] arguments)
            : base(string.Format(formatString, arguments), innerException)
        {
        }

        public static CNotFoundException Create(string objectName)
        {
            return new CNotFoundException("Failed to find '{0}'", new object[1]
            {
                (object) objectName
            });
        }

        public static CNotFoundException Create(
            string objectName,
            Exception innerException)
        {
            return new CNotFoundException(innerException, "Failed to find '{0}'", new object[1]
            {
                (object) objectName
            });
        }
    }
}
