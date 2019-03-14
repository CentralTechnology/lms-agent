using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public static class XmlNamespaceCreator
    {
        public static XmlNamespaceManager Create(XmlDocument document, string prefix)
        {
            if (document == null)
                throw new ArgumentNullException(nameof (document));
            if (document.DocumentElement == null)
                throw new InvalidDataException();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
            foreach (XmlSchema xmlSchema in document.Schemas.Schemas().Cast<XmlSchema>().Where<XmlSchema>((Func<XmlSchema, bool>) (schema => document.DocumentElement.NamespaceURI == schema.TargetNamespace)))
                namespaceManager.AddNamespace(prefix, xmlSchema.TargetNamespace);
            return namespaceManager;
        }
    }
}
