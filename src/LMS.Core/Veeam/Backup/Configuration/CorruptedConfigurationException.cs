using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration
{
    [Serializable]
    internal class CorruptedConfigurationException : Exception
    {
        public CorruptedConfigurationException()
        {
        }

        public CorruptedConfigurationException(string format, params object[] args)
            : this((Exception) null, format, args)
        {
        }

        public CorruptedConfigurationException(
            Exception innerException,
            string format,
            params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        protected CorruptedConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
