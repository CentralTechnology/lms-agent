using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Common
{
    using Serialization;

    public static class CXmlHelper2
  {
    public static bool UseStringInterning { get; set; }

    static CXmlHelper2()
    {
      CXmlHelper2.UseStringInterning = false;
    }

    public static XmlNode AddNode(XmlNode parent, string nodeName, string innerText)
    {
      XmlNode xmlNode = (XmlNode) CXmlHelper2.AddNode(parent, nodeName);
      xmlNode.InnerText = innerText;
      return xmlNode;
    }

    public static XmlElement AddNode(XmlNode parent, string nodeName)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      if (parent is XmlDocument)
        throw new ArgumentException(nameof (parent));
      XmlDocument ownerDocument = parent.OwnerDocument;
      return (XmlElement) parent.AppendChild((XmlNode) ownerDocument.CreateElement(nodeName));
    }

    public static XmlNode FindChildNode(XmlNode node, string childNodeName)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (childNodeName == null)
        throw new ArgumentNullException(nameof (childNodeName));
      return node.ChildNodes.Cast<XmlNode>().FirstOrDefault<XmlNode>((Func<XmlNode, bool>) (childNode => childNode.Name == childNodeName));
    }

    public static XmlNode GetChildNode(XmlNode node, string childNodeName)
    {
      XmlNode childNode = CXmlHelper2.FindChildNode(node, childNodeName);
      if (childNode == null)
        throw new CAppException("Child node {0} was not found", new object[1]
        {
          (object) childNodeName
        });
      return childNode;
    }

    public static XmlNode[] GetChildNodes(XmlNode node, string childNodeName)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (childNodeName == null)
        throw new ArgumentNullException(nameof (childNodeName));
      return node.ChildNodes.Cast<XmlNode>().Where<XmlNode>((Func<XmlNode, bool>) (childNode => childNode.Name == childNodeName)).ToArray<XmlNode>();
    }

    public static XmlNode CreateChildNode(XmlNode node, string childNodeName)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (childNodeName == null)
        throw new ArgumentNullException(nameof (childNodeName));
      XmlDocument ownerDocument = node.OwnerDocument;
      if (ownerDocument == null)
        throw new CAppException("Failed to find node document", new object[0]);
      return (XmlNode) ownerDocument.CreateElement(childNodeName);
    }

    public static void AddAttr(XmlNode node, string nodeName, Enum value)
    {
      string str = Convert.ToString(Convert.ToInt32((object) value), (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, nodeName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, string value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttrIfValueIsExists(XmlNode node, string attrName, string value)
    {
      if (value == null)
        return;
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, Guid value)
    {
      string str = value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, sbyte value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, short value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, int value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, long value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, float value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, double value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, Decimal value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, byte value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, ushort value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, char value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, uint value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, ulong value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, bool value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, TimeSpan value)
    {
      string str = Convert.ToString((object) value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, TimeSpan value, string format)
    {
      string str = value.ToString(format, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, Version value)
    {
      string str = Convert.ToString((object) value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, DateTime value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAttr(XmlNode node, string attrName, DateTimeOffset value)
    {
      string str = Convert.ToString((object) value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAttrInternal(node, attrName, str);
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, string value)
    {
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, bool value)
    {
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, Guid value)
    {
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, Convert.ToString((object) value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, long value)
    {
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, int value)
    {
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, DateTime value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, str);
    }

    public static void AddAsInnerText(XmlNode parentNode, string childNodeName, double value)
    {
      string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
      CXmlHelper2.AddAsInnerTextInternal(parentNode, childNodeName, str);
    }

    public static void AddAsInnerXml(XmlNode parentNode, string childNodeName, string str)
    {
      CXmlHelper2.AddAsInnerXmlInternal(parentNode, childNodeName, str);
    }

    public static void Serialize<T>(XmlElement parent, string tagName, T objectToWrite)
    {
      CXmlHelper2.AddAsInnerXml((XmlNode) parent, tagName, CSerializeHelper.Serialize<T>(objectToWrite));
    }

    public static void SerializeArray<T>(XmlElement parent, string tagName, T[] objectToWrite)
    {
      CXmlHelper2.AddAsInnerXml((XmlNode) parent, tagName, CSerializeHelper.SerializeArray<T>(objectToWrite));
    }

    public static T Deserialize<T>(XmlElement parent, string tagName)
    {
      return CSerializeHelper.Deserialize<T>(CXmlHelper2.GetInnerXml((XmlNode) parent, tagName));
    }

    public static bool TryDeserialize<T>(XmlElement parent, string tagName, out T value)
    {
      string innerXml = CXmlHelper2.FindInnerXml((XmlNode) parent, tagName, (string) null);
      if (innerXml == null)
      {
        value = default (T);
        return false;
      }
      value = CSerializeHelper.Deserialize<T>(innerXml);
      return true;
    }

    public static T TryDeserialize<T>(XmlElement parent, string tagName)
    {
      T obj;
      if (CXmlHelper2.TryDeserialize<T>(parent, tagName, out obj))
        return obj;
      return default (T);
    }

    public static bool TryDeserializeArray<T>(XmlElement parent, string tagName, out T[] value)
    {
      string innerXml = CXmlHelper2.FindInnerXml((XmlNode) parent, tagName, (string) null);
      if (innerXml == null)
      {
        value = (T[]) null;
        return false;
      }
      if (innerXml == string.Empty)
      {
        value = new T[0];
        return true;
      }
      value = CSerializeHelper.DeserializeArray<T>(innerXml);
      return true;
    }

    public static T[] TryDeserializeArray<T>(XmlElement parent, string tagName)
    {
      T[] objArray;
      if (CXmlHelper2.TryDeserializeArray<T>(parent, tagName, out objArray))
        return objArray;
      return new T[0];
    }

    public static void SetAsInnerText(XmlNode parentNode, string childNodeName, string value)
    {
      CXmlHelper2.SetAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SetAsInnerText(XmlNode parentNode, string childNodeName, bool value)
    {
      CXmlHelper2.SetAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SetAsInnerText(XmlNode parentNode, string childNodeName, Guid value)
    {
      CXmlHelper2.SetAsInnerTextInternal(parentNode, childNodeName, Convert.ToString((object) value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SetAsInnerText(XmlNode parentNode, string childNodeName, long value)
    {
      CXmlHelper2.SetAsInnerTextInternal(parentNode, childNodeName, Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SetAsInnerText(XmlNode parentNode, string childNodeName, byte[] value)
    {
      string base64String = Convert.ToBase64String(value);
      CXmlHelper2.SetAsInnerTextInternal(parentNode, childNodeName, base64String);
    }

    public static void AddAsCData(XmlNode parentNode, string childNodeName, string value)
    {
      CXmlHelper.AddElement(parentNode, childNodeName).AppendChild((XmlNode) parentNode.OwnerDocument.CreateCDataSection(value));
    }

    private static void AddAttrInternal(XmlNode node, string attrName, string value)
    {
      XmlAttribute attribute1 = CXmlHelper2.FindAttribute(node, attrName);
      if (attribute1 == null)
      {
        XmlAttribute attribute2 = node.OwnerDocument.CreateAttribute(attrName);
        attribute2.Value = value ?? string.Empty;
        node.Attributes.Append(attribute2);
      }
      else
        attribute1.Value = value;
    }

    private static void AddAsInnerTextInternal(
      XmlNode parentNode,
      string childNodeName,
      string value)
    {
      CXmlHelper.AddElement(parentNode, childNodeName).InnerText = value ?? string.Empty;
    }

    private static void AddAsInnerXmlInternal(
      XmlNode parentNode,
      string childNodeName,
      string value)
    {
      CXmlHelper.AddElement(parentNode, childNodeName).InnerXml = value ?? string.Empty;
    }

    private static void SetAsInnerTextInternal(
      XmlNode parentNode,
      string childNodeName,
      string value)
    {
      (CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName) ?? (XmlNode) CXmlHelper.AddElement(parentNode, childNodeName)).InnerText = value ?? string.Empty;
    }

    public static string GetAttrAsString(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      if (!CXmlHelper2.UseStringInterning)
        return foundValue;
      return string.Intern(foundValue);
    }

    public static string FindAttrAsString(XmlNode node, string attrName, string defValue)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      if (!CXmlHelper2.UseStringInterning)
        return foundValue;
      return string.Intern(foundValue);
    }

    public static T GetAttrAsEnum<T>(XmlNode node, string attrName) where T : struct, IConvertible
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return SEnumExtensions.Parse<T>(foundValue);
    }

    public static bool GetAttrAsBoolean(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToBoolean(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static TimeSpan GetAttrAsTimeSpan(XmlNode node, string attrName)
    {
      TimeSpan? attrAsTimeSpan = CXmlHelper2.FindAttrAsTimeSpan(node, attrName, new TimeSpan?());
      if (!attrAsTimeSpan.HasValue)
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return attrAsTimeSpan.Value;
    }

    public static bool FindAttrAsBoolean(XmlNode node, string attrName, bool defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToBoolean(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static T? FindAttrAsEnum<T>(XmlNode node, string attrName, T? defValue) where T : struct, IConvertible
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      return new T?(SEnumExtensions.Parse<T>(foundValue));
    }

    public static T FindAttrAsEnum<T>(XmlNode node, string attrName, T defValue) where T : struct, IConvertible
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      return SEnumExtensions.Parse<T>(foundValue);
    }

    public static bool? FindAttrAsBoolean(XmlNode node, string attrName, bool? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new bool?(Convert.ToBoolean(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static DateTime GetAttrAsDateTime(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToDateTime(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static DateTime FindAttrAsDateTime(
      XmlNode node,
      string attrName,
      DateTime defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToDateTime(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static TimeSpan? FindAttrAsTimeSpan(
      XmlNode node,
      string attrName,
      TimeSpan? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new TimeSpan?(TimeSpan.Parse(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static DateTime GetAttrAsDateTime(
      XmlNode node,
      string attrName,
      DateTimeKind dateTimeKind)
    {
      return DateTime.SpecifyKind(CXmlHelper2.GetAttrAsDateTime(node, attrName), dateTimeKind);
    }

    public static TimeSpan? FindAttrAsTimeSpan(
      XmlNode node,
      string attrName,
      string format,
      TimeSpan? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new TimeSpan?(TimeSpan.ParseExact(foundValue, format, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static Version GetAttrAsVersion(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Version.Parse(foundValue);
    }

    public static Version FindAttrAsVersion(XmlNode node, string attrName, Version defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Version.Parse(foundValue);
      return defValue;
    }

    public static DateTime? FindAttrAsDateTime(
      XmlNode node,
      string attrName,
      DateTime? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new DateTime?(Convert.ToDateTime(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static DateTime FindAttrAsDateTime(
      XmlNode node,
      string attrName,
      DateTimeKind dateTimeKind,
      DateTime defValue)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      return DateTime.SpecifyKind(Convert.ToDateTime(foundValue, (IFormatProvider) CultureInfo.InvariantCulture), dateTimeKind);
    }

    public static DateTime? FindAttrAsDateTime(
      XmlNode node,
      string attrName,
      DateTimeKind dateTimeKind,
      DateTime? defValue)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      return new DateTime?(DateTime.SpecifyKind(Convert.ToDateTime(foundValue, (IFormatProvider) CultureInfo.InvariantCulture), dateTimeKind));
    }

    public static short GetAttrAsInt16(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToInt16(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static int GetAttrAsInt32(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToInt32(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static Guid GetAttrAsGuid(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return new Guid(foundValue);
    }

    public static byte GetAttrAsByte(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToByte(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static Guid FindAttrAsGuid(XmlNode node, string attrName, Guid defValue)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      return new Guid(foundValue);
    }

    public static Guid? FindAttrAsGuid(XmlNode node, string attrName, Guid? defValue)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return defValue;
      return new Guid?(new Guid(foundValue));
    }

    public static sbyte FindAttrAsSByte(XmlNode node, string attrName, sbyte defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToSByte(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static sbyte? FindAttrAsSByte(XmlNode node, string attrName, sbyte? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new sbyte?(Convert.ToSByte(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static short FindAttrAsInt16(XmlNode node, string attrName, short defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToInt16(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static short? FindAttrAsInt16(XmlNode node, string attrName, short? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new short?(Convert.ToInt16(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static int FindAttrAsInt32(XmlNode node, string attrName, int defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToInt32(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static int? FindAttrAsInt32(XmlNode node, string attrName, int? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new int?(Convert.ToInt32(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static long GetAttrAsInt64(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static long FindAttrAsInt64(XmlNode node, string attrName, long defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static long? FindAttrAsInt64(XmlNode node, string attrName, long? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new long?(Convert.ToInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static float? FindAttrAsSingle(XmlNode node, string attrName, float? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new float?(Convert.ToSingle(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static char GetAttrAsChar(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToChar(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static char? FindAttrAsChar(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new char?();
      return new char?(Convert.ToChar(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static double? FindAttrAsDouble(XmlNode node, string attrName, double? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new double?(Convert.ToDouble(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static Decimal? FindAttrAsDecimal(
      XmlNode node,
      string attrName,
      Decimal? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new Decimal?(Convert.ToDecimal(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static byte? FindAttrAsByte(XmlNode node, string attrName, byte? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new byte?(Convert.ToByte(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static ushort? FindAttrAsUInt16(XmlNode node, string attrName, ushort? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new ushort?(Convert.ToUInt16(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static uint? FindAttrAsUInt32(XmlNode node, string attrName, uint? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new uint?(Convert.ToUInt32(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static uint FindAttrAsUInt32(XmlNode node, string attrName, uint defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToUInt32(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static ulong FindAttrAsUInt64(XmlNode node, string attrName, ulong defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return Convert.ToUInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
      return defValue;
    }

    public static ulong? FindAttrAsUInt64(XmlNode node, string attrName, ulong? defValue)
    {
      string foundValue;
      if (CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new ulong?(Convert.ToUInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
      return defValue;
    }

    public static long? FindAttrAsNullableInt64(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new long?();
      return new long?(Convert.ToInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static bool? FindAttrAsNullableBoolean(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        return new bool?();
      return new bool?(Convert.ToBoolean(foundValue, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static ushort GetAttrAsUInt16(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToUInt16(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static uint GetAttrAsUInt32(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToUInt32(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static ulong GetAttrAsUInt64(XmlNode node, string attrName)
    {
      string foundValue;
      if (!CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue))
        throw new CNotFoundException("XML attribute not found. Node: [{0}], Attr: [{1}]", new object[2]
        {
          (object) node.Name,
          (object) attrName
        });
      return Convert.ToUInt64(foundValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string GetFromCData(XmlNode parentNode, string nodeName)
    {
      if (parentNode == null)
        throw new ArgumentNullException(nameof (parentNode));
      XmlNode xmlNode = parentNode.SelectSingleNode(nodeName);
      XmlCDataSection firstChild = xmlNode.FirstChild as XmlCDataSection;
      if (firstChild == null)
        return xmlNode.InnerText;
      return firstChild.Value;
    }

    public static string FindCData(XmlNode parentNode, string nodeName, string defaultValue)
    {
      XmlNode xmlNode = parentNode.SelectSingleNode(nodeName);
      if (xmlNode == null)
        return defaultValue;
      return (xmlNode.FirstChild as XmlCDataSection).Value;
    }

    public static bool HasAttr(XmlNode node, string attrName)
    {
      string foundValue;
      return CXmlHelper2.TryToGetAttrInternal(node, attrName, out foundValue);
    }

    public static bool HasNode(XmlNode node, string nodeName)
    {
      XmlNode foundNode;
      return CXmlHelper2.TryToGetNodeInternal(node, nodeName, out foundNode);
    }

    public static bool TryToGetNodeInternal(
      XmlNode parentNode,
      string nodeName,
      out XmlNode foundNode)
    {
      if (parentNode == null)
        throw new ArgumentNullException(nameof (parentNode));
      if (nodeName == null)
        throw new ArgumentNullException(nameof (nodeName));
      foundNode = (XmlNode) null;
      if (parentNode.Attributes == null)
        return false;
      foreach (XmlNode childNode in parentNode.ChildNodes)
      {
        if (string.Equals(childNode.Name, nodeName, StringComparison.InvariantCultureIgnoreCase))
        {
          foundNode = childNode;
          return true;
        }
      }
      return false;
    }

    private static bool TryToGetAttrInternal(XmlNode node, string attrName, out string foundValue)
    {
      foundValue = (string) null;
      XmlAttribute attribute = CXmlHelper2.FindAttribute(node, attrName);
      if (attribute == null)
        return false;
      foundValue = attribute.Value;
      return true;
    }

    private static XmlAttribute FindAttribute(XmlNode node, string attrName)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (attrName == null)
        throw new ArgumentNullException(nameof (attrName));
      if (node.Attributes == null)
        return (XmlAttribute) null;
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) node.Attributes)
      {
        if (string.Equals(attribute.Name, attrName, StringComparison.InvariantCultureIgnoreCase))
          return attribute;
      }
      return (XmlAttribute) null;
    }

    public static void ConvertValue(string xmlValue, out uint value)
    {
      value = Convert.ToUInt32(xmlValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static void ConvertValue(string xmlValue, out bool value)
    {
      value = Convert.ToBoolean(xmlValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static byte[] GetInnerTextAsBytes(XmlNode node, string nodeName)
    {
      return Convert.FromBase64String(CXmlHelper2.GetInnerText(node, nodeName));
    }

    public static bool GetInnerTextAsBoolean(XmlNode node, string nodeName)
    {
      return Convert.ToBoolean(CXmlHelper2.GetInnerText(node, nodeName), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static uint GetInnerTextAsUInt32(XmlNode node, string nodeName)
    {
      return Convert.ToUInt32(CXmlHelper2.GetInnerText(node, nodeName), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static int GetInnerTextAsInt32(XmlNode node, string nodeName)
    {
      return Convert.ToInt32(CXmlHelper2.GetInnerText(node, nodeName), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static Guid GetInnerTextAsGuid(XmlNode node, string nodeName)
    {
      return new Guid(CXmlHelper2.GetInnerText(node, nodeName));
    }

    public static long GetInnerTextAsInt64(XmlNode node, string nodeName)
    {
      return Convert.ToInt64(CXmlHelper2.GetInnerText(node, nodeName), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static DateTime GetInnerTextAsDateTime(XmlNode node, string nodeName)
    {
      return Convert.ToDateTime(CXmlHelper2.GetInnerText(node, nodeName), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string GetInnerText(XmlNode node, string nodeName)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        throw new CNotFoundException("Xml node is not found. Name: [{0}]", new object[1]
        {
          (object) nodeName
        });
      return innerText;
    }

    public static string GetInnerXml(XmlNode node, string nodeName)
    {
      string innerXml = CXmlHelper2.FindInnerXml(node, nodeName, (string) null);
      if (innerXml == null)
        throw new CNotFoundException("Xml node is not found. Name: [{0}]", new object[1]
        {
          (object) nodeName
        });
      return innerXml;
    }

    public static string FindInnerText(XmlNode parentNode, string nodeName, string defaultValue)
    {
      XmlNode xmlNode = parentNode.SelectSingleNode(nodeName);
      if (xmlNode == null)
        return defaultValue;
      return xmlNode.InnerText;
    }

    public static string FindInnerXml(XmlNode parentNode, string nodeName, string defaultValue)
    {
      XmlNode xmlNode = parentNode.SelectSingleNode(nodeName);
      if (xmlNode == null)
        return defaultValue;
      return xmlNode.InnerXml;
    }

    public static Guid FindInnerText(XmlNode node, string nodeName, Guid defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return new Guid(innerText);
    }

    public static bool FindInnerText(XmlNode node, string nodeName, bool defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return Convert.ToBoolean(innerText, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static uint FindInnerText(XmlNode node, string nodeName, uint defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return Convert.ToUInt32(innerText, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static ulong FindInnerText(XmlNode node, string nodeName, ulong defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return Convert.ToUInt64(innerText, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static int FindInnerText(XmlNode node, string nodeName, int defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return Convert.ToInt32(innerText, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static double FindInnerText(XmlNode node, string nodeName, double defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return Convert.ToDouble(innerText, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static int? FindInnerTextAsNullableInt32(
      XmlNode node,
      string nodeName,
      int? defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      int result;
      if (innerText == null || !int.TryParse(innerText, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return defaultValue;
      return new int?(result);
    }

    public static double? FindInnerTextAsNullableDouble(
      XmlNode node,
      string nodeName,
      double? defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      double result;
      if (innerText == null || !double.TryParse(innerText, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return defaultValue;
      return new double?(result);
    }

    public static DateTime FindInnerText(
      XmlNode node,
      string nodeName,
      DateTime defaultValue)
    {
      string innerText = CXmlHelper2.FindInnerText(node, nodeName, (string) null);
      if (innerText == null)
        return defaultValue;
      return Convert.ToDateTime(innerText, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static XmlNode GetSingleNode(XmlNode node, string query)
    {
      XmlNode singleNode = CXmlHelper2.FindSingleNode(node, query);
      if (singleNode == null)
        throw new CNotFoundException("Failed to find child node. Query: [{0}]", new object[1]
        {
          (object) query
        });
      return singleNode;
    }

    public static XmlNode FindSingleNode(XmlNode node, string query)
    {
      return node.SelectSingleNode(query);
    }

    public static XmlNodeList GetNodeList(XmlNode node, string query)
    {
      return node.SelectNodes(query);
    }

    public static XmlNodeList GetNonEmptyNodeList(XmlNode node, string query)
    {
      XmlNodeList nodeList = CXmlHelper2.GetNodeList(node, query);
      if (nodeList.Count == 0)
        throw new CNotFoundException("Failed to find child nodes. Query: [{0}]", new object[1]
        {
          (object) query
        });
      return nodeList;
    }

    public static void SetAttr(XmlNode node, string attrName, string value)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (attrName == null)
        throw new ArgumentNullException(nameof (attrName));
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) node.Attributes)
      {
        if (string.Equals(attribute.Name, attrName, StringComparison.InvariantCultureIgnoreCase))
        {
          attribute.Value = value;
          return;
        }
      }
      throw new CAppException("Xml attribute [{0}] was not found. You can use AddAttr for add new attribute", new object[1]
      {
        (object) attrName
      });
    }

    public static XmlDocument CreateDoc(string rootElementName)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(string.Format("<?xml version=\"1.0\"?><{0}/>", (object) rootElementName));
      return xmlDocument;
    }

    public static XmlElement GetDocumentRootElement(XmlDocument xmlDocument)
    {
      XmlElement documentElement = xmlDocument.DocumentElement;
      if (documentElement == null)
        throw new CAppException("Root element is null", new object[0]);
      return documentElement;
    }

    public static XmlDocument LoadDoc(string xml, string rootElemName)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xml);
      if (rootElemName != null)
      {
        if (xmlDocument.DocumentElement == null)
          throw new CAppException("There is no root element.", new object[0]);
        if (xmlDocument.DocumentElement.Name != rootElemName)
          throw new CAppException("Root element's name is invalid. ElemName: [{0}], Expected: [{1}]", new object[2]
          {
            (object) xmlDocument.DocumentElement.Name,
            (object) rootElemName
          });
      }
      return xmlDocument;
    }

    public static XmlNode ImportNode(XmlNode parentNode, XmlNode importedNode)
    {
      XmlNode xmlNode = (XmlNode) null;
      if (importedNode != null)
        xmlNode = parentNode.AppendChild(parentNode.OwnerDocument.ImportNode<XmlNode>(importedNode));
      return xmlNode;
    }

    public static void AddArray(XmlNode node, string valueNodeName, string[] values)
    {
      foreach (string innerText in values)
        CXmlHelper2.AddNode(node, valueNodeName, innerText);
    }

    public static string[] GetAsStringArray(XmlNode node, string valueNodeName)
    {
      List<string> stringList = new List<string>();
      foreach (XmlNode node1 in CXmlHelper2.GetNodeList(node, valueNodeName))
        stringList.Add(node1.InnerText);
      return stringList.ToArray();
    }

    public static void AddArray(XmlNode node, string valueNodeName, Guid[] values)
    {
      foreach (Guid guid in values)
        CXmlHelper2.AddNode(node, valueNodeName, guid.ToString());
    }

    public static Guid[] GetAsGuidArray(XmlNode node, string valueNodeName)
    {
      List<Guid> guidList = new List<Guid>();
      foreach (XmlNode node1 in CXmlHelper2.GetNodeList(node, valueNodeName))
      {
        Guid result;
        if (!Guid.TryParse(node1.InnerText, out result))
          throw new FormatException("Unable to parse string to GUID");
        guidList.Add(result);
      }
      return guidList.ToArray();
    }

    public static string RemoveInvalidCharacters(string value)
    {
      if (string.IsNullOrEmpty(value))
        return value;
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      foreach (char ch in value)
      {
        if (CXmlHelper2.IsValidCharacter((int) ch))
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }

    public static bool IsValidCharacter(int ch)
    {
      if (ch == 9 || ch == 10 || ch == 13 || (ch >= 32 && ch <= 55295 || ch >= 57344 && ch <= 65533))
        return true;
      if (ch >= 65536)
        return ch <= 1114111;
      return false;
    }
  }
}
