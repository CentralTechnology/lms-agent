using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LMS.Core.Common;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class CXmlReaderExm
  {
    public static byte[] GetBytes(this XmlReader xmlReader, string attributeName)
    {
      return Convert.FromBase64String(xmlReader.GetAttributeString(attributeName));
    }

    public static byte[] FindBytes(this XmlReader xmlReader, string attributeName)
    {
      string attributeString = xmlReader.FindAttributeString(attributeName);
      if (string.IsNullOrEmpty(attributeString))
        return new byte[0];
      return Convert.FromBase64String(attributeString);
    }

    public static Guid GetGuid(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<Guid>(attributeName);
    }

    public static Guid? FindGuid(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.FindAttribute<Guid>(attributeName);
    }

    public static Guid FindGuid(
      this XmlReader xmlReader,
      string attributeName,
      Guid defaultValue)
    {
      return xmlReader.FindAttribute<Guid>(attributeName, defaultValue);
    }

    public static bool FindBool(this XmlReader xmlReader, string attributeName, bool defaultValue)
    {
      return xmlReader.FindAttribute<bool>(attributeName, defaultValue);
    }

    public static bool? FindBool(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.FindAttribute<bool>(attributeName);
    }

    public static bool GetBool(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<bool>(attributeName);
    }

    public static ulong GetUInt64(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<ulong>(attributeName);
    }

    public static int GetInt32(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<int>(attributeName);
    }

    public static string GetString(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttributeString(attributeName);
    }

    public static DateTime GetDateTime(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<DateTime>(attributeName);
    }

    public static DateTime GetDateTime(
      this XmlReader xmlReader,
      string attributeName,
      DateTimeKind dateTimeKind)
    {
      return DateTime.SpecifyKind(xmlReader.GetAttribute<DateTime>(attributeName), dateTimeKind);
    }

    public static long GetInt64(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<long>(attributeName);
    }

    public static Decimal GetDecimal(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.GetAttribute<Decimal>(attributeName);
    }

    public static long? FindInt64(this XmlReader xmlReader, string attributeName)
    {
      return xmlReader.FindAttribute<long>(attributeName);
    }

    public static bool IsEndElementOf(this XmlReader xmlReader, string nodeName)
    {
      if (xmlReader.NodeType == XmlNodeType.EndElement)
        return string.Compare(xmlReader.Name, nodeName, StringComparison.InvariantCultureIgnoreCase) == 0;
      return false;
    }

    public static bool IsStartElementOf(this XmlReader xmlReader, string nodeName)
    {
      if (xmlReader.NodeType == XmlNodeType.Element)
        return string.Compare(xmlReader.Name, nodeName, StringComparison.InvariantCultureIgnoreCase) == 0;
      return false;
    }

    public static T? FindAttribute<T>(this XmlReader reader, string attrName) where T : struct
    {
      string attribute = reader.GetAttribute(attrName);
      if (attribute == null)
        return new T?();
      return new T?(CXmlReaderExm.FromString<T>(attribute));
    }

    public static T FindAttribute<T>(this XmlReader reader, string attrName, T defaultValue) where T : struct
    {
      string attribute = reader.GetAttribute(attrName);
      if (attribute == null)
        return defaultValue;
      return CXmlReaderExm.FromString<T>(attribute);
    }

    public static T GetAttribute<T>(this XmlReader reader, string attrName) where T : struct
    {
      string attribute = reader.GetAttribute(attrName);
      if (attribute == null)
        throw ExceptionFactory.Create("Attribute '{0}' was not found", (object) attrName);
      return CXmlReaderExm.FromString<T>(attribute);
    }

    public static string GetAttributeString(this XmlReader reader, string attrName)
    {
      string attribute = reader.GetAttribute(attrName);
      if (attribute == null)
        throw ExceptionFactory.Create("Attribute '{0}' was not found", (object) attrName);
      return attribute;
    }

    public static string FindAttributeString(this XmlReader reader, string attrName)
    {
      return reader.GetAttribute(attrName);
    }

    public static string FindAttributeString(
      this XmlReader reader,
      string attrName,
      string defaultValue)
    {
      return reader.GetAttribute(attrName) ?? defaultValue;
    }

    private static T FromString<T>(string value) where T : struct
    {
      System.Type conversionType = typeof (T);
      if (conversionType == typeof (Guid))
        return (T) (ValueType) Guid.Parse(value);
      return (T) Convert.ChangeType((object) value, conversionType, (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
