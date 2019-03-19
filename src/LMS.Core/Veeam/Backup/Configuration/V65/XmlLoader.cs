using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public class XmlLoader
    {
        private readonly string[] _xmlSchemas;

        public XmlLoader(params string[] xmlSchemas)
        {
            if (xmlSchemas == null)
                throw new ArgumentNullException(nameof (xmlSchemas));
            this._xmlSchemas = xmlSchemas;
        }

        public XmlDocument LoadFromFile(string path)
        {
            return this.LoadFromContent(File.ReadAllText(path));
        }

        public XmlDocument LoadFromContent(string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent))
                throw new ArgumentNullException(nameof (xmlContent));
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ValidationType = ValidationType.Schema
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (ValidationEventHandler) ((sender, args) =>
            {
                switch (args.Severity)
                {
                    case XmlSeverityType.Error:
                        throw args.Exception;
                    case XmlSeverityType.Warning:
                        throw new XmlSchemaValidationException(args.Message);
                    default:
                        throw new InvalidEnumArgumentException();
                }
            });
            foreach (string xmlSchema in this._xmlSchemas)
            {
                using (StringReader stringReader = new StringReader(xmlSchema))
                {
                    using (XmlReader schemaDocument = XmlReader.Create((TextReader) stringReader))
                        settings.Schemas.Add((string) null, schemaDocument);
                }
            }
            XmlDocument xmlDocument = new XmlDocument();
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                using (XmlReader reader = XmlReader.Create((TextReader) stringReader, settings))
                    xmlDocument.Load(reader);
            }
            return xmlDocument;
        }
    }
}
