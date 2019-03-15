using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using LMS.Core.Veeam.Backup.Common;

namespace LMS.Core.Common
{
    public static class ExceptionFactory
    {
        public static Exception Create(string message)
        {
            return new Exception(message);
        }

        public static Exception Create(Exception innerException, string message)
        {
            return new Exception(message, innerException);
        }

        [StringFormatMethod("format")]
        public static Exception Create(string format, params object[] args)
        {
            return new Exception(string.Format(format, args));
        }

        [StringFormatMethod("format")]
        public static Exception Create(
            Exception innerException,
            string format,
            params object[] args)
        {
            return new Exception(string.Format(format, args), innerException);
        }

        public static UnsupportedVersionException CreateUnsupportedVersionException(string format, params object[] args)
        {
            return new UnsupportedVersionException(string.Format(format, args));
        }

        public static XmlException XmlException(string format, params object[] args)
        {
            return new XmlException(string.Format(format, args));
        }

        public static Exception ThrowNecessaryAggregateException(
            string message,
            IEnumerable<Exception> exceptionsCollection)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(message);
            List<Exception> exceptionList = new List<Exception>();
            foreach (Exception exceptions in exceptionsCollection)
            {
                exceptionList.Add(exceptions);
                stringBuilder.AppendLine(exceptions.Message);
            }
            if (exceptionList.Count == 0)
                throw new ArgumentEmptyException(nameof (exceptionsCollection));
            if (exceptionList.Count == 1)
                throw new Exception(message, exceptionList[0]);
            throw new AggregateException(stringBuilder.ToString(), (IEnumerable<Exception>) exceptionList);
        }
    }
}
