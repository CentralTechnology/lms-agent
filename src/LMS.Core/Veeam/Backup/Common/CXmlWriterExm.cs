using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class CXmlWriterExm
    {
        public static void SetAttribute(this XmlWriter writer, string attributeName, byte[] value)
        {
            string base64String = Convert.ToBase64String(value);
            writer.WriteAttributeString(attributeName, base64String);
        }

        public static void SetAttribute<T>(this XmlWriter writer, string attributeName, T value) where T : struct
        {
            string str = CXmlWriterExm.ToString<T>(value);
            writer.WriteAttributeString(attributeName, str);
        }

        public static void SetAttribute(this XmlWriter writer, string attributeName, string value)
        {
            writer.WriteAttributeString(attributeName, value);
        }

        public static void SetAttributeIfNotNullorEmpty(
            this XmlWriter writer,
            string attributeName,
            string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            writer.WriteAttributeString(attributeName, value);
        }

        private static string ToString<T>(T value) where T : struct
        {
            if (typeof (T) == typeof (Guid))
                return value.ToString();
            return Convert.ToString((object) value, (IFormatProvider) CultureInfo.InvariantCulture);
        }
    }
}
