using System;
using System.Runtime.Serialization;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class TerminatedException : Exception
    {
        public const string ExceptionMessage = "Terminated";

        public TerminatedException(string message)
            : base(string.IsNullOrEmpty(message) ? "Terminated" : message)
        {
        }

        protected TerminatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
