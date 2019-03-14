using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Serialization.Xml
{
  internal static class XmlExtensionMethods
  {
    internal static void ExpectElement(this XmlReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new InvalidDataException(reader.NodeType.ToString());
    }

    internal static bool MoveToNextElement(this XmlReader reader)
    {
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element)
          return true;
      }
      return false;
    }

    internal static void MoveToNextExpectedElement(this XmlReader reader, string elementTag)
    {
      reader.MoveToNextExpectedElement(elementTag, string.Empty);
    }

    internal static void MoveToNextExpectedElement(
      this XmlReader reader,
      string elementTag,
      string namespaceUri)
    {
      if (!reader.MoveToNextElement())
        throw new InvalidDataException("There is no an expected attribute: [" + elementTag + "]");
      if (reader.LocalName != elementTag || reader.NamespaceURI != namespaceUri)
        throw new InvalidDataException(string.Format("An unexpected element has occurred: [{0}:{1}], expected: [{2}:{3}].", (object) reader.NamespaceURI, (object) reader.LocalName, (object) namespaceUri, (object) elementTag));
    }

    internal static bool MoveToEndOfElement(this XmlReader reader)
    {
      return reader.MoveToContent() == XmlNodeType.EndElement;
    }

    internal static void ExpectAttribute(this XmlReader reader)
    {
      if (reader.NodeType != XmlNodeType.Attribute)
        throw new InvalidDataException(reader.NodeType.ToString());
    }

    internal static void ExpectAttribute(
      this XmlReader reader,
      string attributeTag,
      string namespaceUri)
    {
      if (!reader.IsExpectedAttribute(attributeTag, namespaceUri))
        throw new InvalidDataException(string.Format("An unexpected attribute has occurred: [{0}:{1}], expected: [{2}:{3}].", (object) reader.NamespaceURI, (object) reader.LocalName, (object) namespaceUri, (object) attributeTag));
    }

    internal static bool IsExpectedAttribute(
      this XmlReader reader,
      string attributeTag,
      string namespaceUri)
    {
      if (reader.NodeType != XmlNodeType.Attribute)
        throw new InvalidDataException(string.Format("An unexpected node [{0}:{1}] of type [{2}] has occurred, expected attribute: [{3}:{4}].", (object) reader.NamespaceURI, (object) reader.LocalName, (object) reader.NodeType, (object) namespaceUri, (object) attributeTag));
      if (reader.LocalName == attributeTag)
        return reader.NamespaceURI == namespaceUri;
      return false;
    }

    internal static Exception CreateUnexpectNodeException(this XmlReader reader)
    {
      throw new InvalidDataException(string.Format("An unexpected {0} has occurred: [{1}:{2}].", (object) reader.NodeType, (object) reader.NamespaceURI, (object) reader.LocalName));
    }

    internal static void MoveToNextExpectedAttribute(this XmlReader reader, string attributeTag)
    {
      reader.MoveToNextExpectedAttribute(attributeTag, string.Empty);
    }

    internal static void MoveToNextExpectedAttribute(
      this XmlReader reader,
      string attributeTag,
      string namespaceUri)
    {
      if (!reader.MoveToNextAttribute(true))
        throw new InvalidDataException("There is no an expected attribute: [" + attributeTag + "]");
      reader.ExpectAttribute(attributeTag, namespaceUri);
    }

    internal static bool TryMoveToNextExpectedAttribute(
      this XmlReader reader,
      string attributeTag,
      string namespaceUri)
    {
      if (!reader.MoveToNextAttribute(true))
        return false;
      reader.ExpectAttribute(attributeTag, namespaceUri);
      return true;
    }

    internal static bool MoveToNextAttribute(this XmlReader reader, bool skipXmlnsAttributes)
    {
      while (reader.MoveToNextAttribute())
      {
        if (!skipXmlnsAttributes || reader.Prefix != "xmlns")
          return true;
      }
      return false;
    }

    internal static XmlReader MoveToSubtree(this XmlReader reader)
    {
      reader.MoveToElement();
      XmlReader xmlReader = reader.ReadSubtree();
      xmlReader.Read();
      return xmlReader;
    }

    internal static bool TryWriteNull(this XmlWriter xw, object value)
    {
      if (!object.ReferenceEquals(value, (object) null))
        return false;
      xw.WriteStartElement("Null", "vxs");
      xw.WriteEndElement();
      return true;
    }

    internal static bool IsNullObject(this XmlReader xr)
    {
      if (xr.NodeType == XmlNodeType.Element && xr.LocalName == "Null")
        return xr.NamespaceURI == "vxs";
      return false;
    }
  }
}
