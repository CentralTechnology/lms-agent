using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Serilization
{
    public static class CSerializeHelper
    {
        public static string Serialize<T>(T objectToWrite)
        {
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (XmlWriter xw = XmlWriter.Create(output, settings))
            {
                xw.SerializeObject(typeof (T), (object) objectToWrite);
                xw.Flush();
            }
            return output.ToString();
        }

        public static string SerializeArray<T>(T[] objectsToWrite)
        {
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                writer.SerializeArray<T>(objectsToWrite);
                writer.Flush();
            }
            return output.ToString();
        }

        public static T Deserialize<T>(string str)
        {
            using (StringReader stringReader = new StringReader(str))
                return (T) XmlReader.Create((TextReader) stringReader).DeserializeObject(typeof (T));
        }

        public static T[] DeserializeArray<T>(string str)
        {
            using (StringReader stringReader = new StringReader(str))
                return (T[]) XmlReader.Create((TextReader) stringReader).DeserializeArray(typeof (T[]), typeof (T));
        }
    }
}
