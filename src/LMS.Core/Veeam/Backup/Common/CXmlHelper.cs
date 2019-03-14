using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class CXmlHelper
  {
    private const string CdataPrefix = "<![CDATA[";
    private const string CdataPostfix = "]]>";

    public static bool UseStringInterning { get; set; }

    static CXmlHelper()
    {
      CXmlHelper.UseStringInterning = false;
    }

    public static XmlElement EnsureRootExistsSafe(string xml, string root)
    {
      Exceptions.CheckArgumentNullOrEmptyException(root, nameof (root));
      try
      {
        XmlDocument node = new XmlDocument();
        if (string.IsNullOrEmpty(xml))
          return node.EnsureElementExists(root);
        node.LoadXml(xml);
        return node.DocumentElement;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Failed to load xml document. Creating new... [root: {0}, xml: {1}]", (object) root, (object) (xml ?? string.Empty));
        return new XmlDocument().EnsureElementExists(root);
      }
    }

    public static XmlElement AddElement(XmlDocument doc, string elementName)
    {
      return (XmlElement) doc.AppendChild((XmlNode) doc.CreateElement(elementName));
    }

    public static XmlElement AddElement(XmlNode node, string elementName)
    {
      return (XmlElement) node.AppendChild((XmlNode) CXmlHelper.CreateElement(node, elementName));
    }

    public static XmlElement CreateElement(XmlNode node, string elementName)
    {
      return (node as XmlDocument ?? node.OwnerDocument).CreateElement(elementName);
    }

    public static XmlAttribute AddAttr(XmlNode node, string attrName, string attrValue)
    {
      return node.Attributes.Append(CXmlHelper.CreateAttr(node.OwnerDocument, attrName, attrValue));
    }

    public static XmlAttribute SetAttr(XmlNode node, string attrName, string attrValue)
    {
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) node.Attributes)
      {
        if (string.Compare(attribute.Name, attrName, true) == 0)
        {
          attribute.Value = attrValue;
          return attribute;
        }
      }
      return CXmlHelper.AddAttr(node, attrName, attrValue);
    }

    public static XmlAttribute CreateAttr(
      XmlDocument doc,
      string attrName,
      string attrValue)
    {
      XmlAttribute attribute = doc.CreateAttribute(attrName);
      attribute.Value = attrValue;
      return attribute;
    }

    public static string FindAttrValue(XmlNode node, string attrName)
    {
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) node.Attributes)
      {
        if (string.Compare(attribute.Name, attrName, true) == 0)
          return CXmlHelper.UseStringInterning ? string.Intern(attribute.Value) : attribute.Value;
      }
      return (string) null;
    }

    public static string GetAttrValue(XmlNode node, string attrName)
    {
      try
      {
        string str = node.Attributes[attrName].Value;
        return CXmlHelper.UseStringInterning ? string.Intern(str) : str;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not defined.", (object) attrName);
        throw;
      }
    }

    public static bool GetBoolAttrValue(XmlNode node, string attrName)
    {
      return CXmlHelper.StrToBool(CXmlHelper.GetAttrValue(node, attrName));
    }

    public static bool GetBoolAttrValue(XmlNode node, string attrName, bool defaultValue)
    {
      string attrValue = CXmlHelper.FindAttrValue(node, attrName);
      if (string.IsNullOrEmpty(attrValue))
        return defaultValue;
      return CXmlHelper.StrToBool(attrValue);
    }

    public static int GetInt32AttrValue(XmlNode node, string attrName, int defaultValue)
    {
      string attrValue = CXmlHelper.FindAttrValue(node, attrName);
      if (string.IsNullOrEmpty(attrValue))
        return defaultValue;
      return Convert.ToInt32(attrValue);
    }

    public static int GetIntAttrValue(XmlNode node, string attrName)
    {
      string attrValue = CXmlHelper.GetAttrValue(node, attrName);
      try
      {
        return int.Parse(attrValue);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not an integer ('{1}').", (object) attrName, (object) attrValue);
        throw;
      }
    }

    public static long GetInt64AttrValue(XmlNode node, string attrName)
    {
      string attrValue = CXmlHelper.GetAttrValue(node, attrName);
      try
      {
        return long.Parse(attrValue);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not an long ('{1}').", (object) attrName, (object) attrValue);
        throw;
      }
    }

    public static uint GetUIntAttrValue(XmlNode node, string attrName)
    {
      string attrValue = CXmlHelper.GetAttrValue(node, attrName);
      try
      {
        return uint.Parse(attrValue);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not an unsigned integer ('{1}').", (object) attrName, (object) attrValue);
        throw;
      }
    }

    public static ulong GetUInt64AttrValue(XmlNode node, string attrName)
    {
      string attrValue = CXmlHelper.GetAttrValue(node, attrName);
      try
      {
        return ulong.Parse(attrValue);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not an unsigned long ('{1}').", (object) attrName, (object) attrValue);
        throw;
      }
    }

    public static Guid GetGuidAttrValue(XmlNode node, string attrName)
    {
      string attrValue = CXmlHelper.GetAttrValue(node, attrName);
      try
      {
        return new Guid(attrValue);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not a GUID value ('{1}').", (object) attrName, (object) attrValue);
        throw;
      }
    }

    public static string GetAttrValue(XmlNode node, string attrName, string defaultValue)
    {
      return CXmlHelper.FindAttrValue(node, attrName) ?? defaultValue;
    }

    public static XmlNode AddChildNode(
      XmlNode parentNode,
      string childNodeName,
      string childNodeValue)
    {
      try
      {
        XmlNode xmlNode = (XmlNode) CXmlHelper.AddElement(parentNode, childNodeName);
        xmlNode.InnerXml = childNodeValue ?? string.Empty;
        return xmlNode;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to add XML node [{0}] with  value '{1}'.", (object) childNodeName, (object) childNodeValue);
        throw;
      }
    }

    public static XmlNode SecureAddChildNode(
      XmlNode parentNode,
      string childNodeName,
      string childNodeValue)
    {
      try
      {
        XmlNode xmlNode = (XmlNode) CXmlHelper.AddElement(parentNode, childNodeName);
        xmlNode.InnerXml = Convert.ToBase64String(Encoding.UTF8.GetBytes(childNodeValue ?? string.Empty));
        return xmlNode;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to add XML node [{0}] with secured value.", (object) childNodeName);
        throw;
      }
    }

    public static XmlNode AddChildNodeText(
      XmlNode parentNode,
      string childNodeName,
      string childNodeValue)
    {
      try
      {
        XmlNode xmlNode = (XmlNode) CXmlHelper.AddElement(parentNode, childNodeName);
        xmlNode.InnerText = childNodeValue ?? string.Empty;
        return xmlNode;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to add XML node [{0}] with  value '{1}'.", (object) childNodeName, (object) childNodeValue);
        throw;
      }
    }

    public static void SetChildNodeText(
      XmlNode parentNode,
      string childNodeName,
      string childNodeValue)
    {
      try
      {
        (CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName) ?? (XmlNode) CXmlHelper.AddElement(parentNode, childNodeName)).InnerText = childNodeValue ?? string.Empty;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to set XML node [{0}] value '{1}'.", (object) childNodeName, (object) childNodeValue);
        throw;
      }
    }

    public static XmlNode AddArrayToXml(
      XmlNode parentNode,
      string arrayNodeName,
      IEnumerable<string> array,
      bool code = false)
    {
      return CXmlHelper.AddArrayToXml<string>(parentNode, arrayNodeName, "string", array.ToArray<string>(), code);
    }

    public static string[] GetArrayFromXml(XmlNode parentNode, string arrayNodeName, bool decode = false)
    {
      return CXmlHelper.GetArrayFromXml(parentNode, arrayNodeName, "string", new string[0], decode);
    }

    public static XmlNode AddArrayToXml<T>(
      XmlNode parentNode,
      string arrayNodeName,
      string arrayElemName,
      T[] array,
      bool code = false)
    {
      XmlNode parentNode1 = CXmlHelper.FindFirstChildNodeByName(parentNode, arrayNodeName) ?? (XmlNode) CXmlHelper.AddElement(parentNode, arrayNodeName);
      foreach (T obj in array)
      {
        string str = obj.ToString();
        if (code)
          str = CXmlHelper.CodeText(str);
        CXmlHelper.AddChildNode(parentNode1, arrayElemName, str);
      }
      return parentNode1;
    }

    public static void SetArrayToXml<T>(
      XmlNode parentNode,
      string arrayNodeName,
      string arrayElemName,
      T[] array,
      bool code = false)
    {
      foreach (XmlNode childNode in parentNode.ChildNodes)
      {
        if (childNode.Name == arrayNodeName)
          parentNode.RemoveChild(childNode);
      }
      XmlNode parentNode1 = (XmlNode) CXmlHelper.AddElement(parentNode, arrayNodeName);
      foreach (T obj in array)
      {
        string str = obj.ToString();
        if (code)
          str = CXmlHelper.CodeText(str);
        CXmlHelper.AddChildNode(parentNode1, arrayElemName, str);
      }
    }

    public static string[] GetArrayFromXml(
      XmlNode parentNode,
      string arrayNodeName,
      string arrayElemName,
      string[] defaulValue,
      bool decode = false)
    {
      string[] arrayFromXml = CXmlHelper.GetArrayFromXml<string>(parentNode, arrayNodeName, arrayElemName, defaulValue, decode);
      if (!CXmlHelper.UseStringInterning)
        return arrayFromXml;
      if (arrayFromXml != null)
        return ((IEnumerable<string>) arrayFromXml).Select<string, string>(new Func<string, string>(string.Intern)).ToArray<string>();
      return (string[]) null;
    }

    public static T[] GetArrayFromXml<T>(
      XmlNode parentNode,
      string arrayNodeName,
      string arrayElemName,
      T[] defaulValue,
      bool decode = false)
    {
      XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, arrayNodeName);
      if (firstChildNodeByName == null)
        return defaulValue;
      XmlNode[] childNodesByName = CXmlHelper.FindChildNodesByName(firstChildNodeByName, arrayElemName);
      if (childNodesByName.Length == 0)
        return defaulValue;
      List<T> objList = new List<T>(childNodesByName.Length);
      foreach (XmlNode xmlNode in childNodesByName)
      {
        string str = xmlNode.InnerXml;
        if (decode)
          str = CXmlHelper.DecodeText(str);
        objList.Add(CXmlHelper.ConvertFromString<T>(str));
      }
      return objList.ToArray();
    }

    public static T SecureGetChildNodeValue<T>(
      XmlNode parentNode,
      string childNodeName,
      T defaultValue)
    {
      try
      {
        XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName);
        T obj = defaultValue;
        if (firstChildNodeByName != null)
          obj = CXmlHelper.ConvertFromString<T>(Encoding.UTF8.GetString(Convert.FromBase64String(firstChildNodeByName.InnerXml)));
        return obj;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}] in [{1}]", (object) childNodeName, (object) parentNode.Name);
        throw;
      }
    }

    public static T GetChildNodeValue<T>(XmlNode parentNode, string childNodeName, T defaultValue)
    {
      return CXmlHelper.GetChildNodeValue<T>(parentNode, childNodeName, defaultValue, false);
    }

    public static string GetChildNodeValue(
      XmlNode parentNode,
      string childNodeName,
      string defaultValue)
    {
      string childNodeValue = CXmlHelper.GetChildNodeValue<string>(parentNode, childNodeName, defaultValue, false);
      if (!CXmlHelper.UseStringInterning)
        return childNodeValue;
      if (childNodeValue != null)
        return string.Intern(childNodeValue);
      return (string) null;
    }

    public static string GetChildNodeValue(
      XmlNode parentNode,
      string childNodeName,
      string defaultValue,
      bool decodeText)
    {
      string childNodeValue = CXmlHelper.GetChildNodeValue<string>(parentNode, childNodeName, defaultValue, decodeText);
      if (!CXmlHelper.UseStringInterning)
        return childNodeValue;
      if (childNodeValue != null)
        return string.Intern(childNodeValue);
      return (string) null;
    }

    public static T GetChildNodeValue<T>(
      XmlNode parentNode,
      string childNodeName,
      T defaultValue,
      bool decodeText)
    {
      try
      {
        XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName);
        T obj = defaultValue;
        if (firstChildNodeByName != null)
        {
          string str = firstChildNodeByName.InnerXml;
          if (decodeText)
            str = CXmlHelper.DecodeText(str);
          obj = CXmlHelper.ConvertFromString<T>(str);
        }
        return obj;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}] in [{1}]", (object) childNodeName, (object) parentNode.Name);
        throw;
      }
    }

    public static string GetChildNodeText(
      XmlNode parentNode,
      string childNodeName,
      string defaultValue)
    {
      string childNodeText = CXmlHelper.GetChildNodeText<string>(parentNode, childNodeName, defaultValue);
      if (!CXmlHelper.UseStringInterning)
        return childNodeText;
      if (childNodeText != null)
        return string.Intern(childNodeText);
      return (string) null;
    }

    public static T GetChildNodeText<T>(XmlNode parentNode, string childNodeName, T defaultValue)
    {
      try
      {
        XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName);
        T obj = defaultValue;
        if (firstChildNodeByName != null)
          obj = CXmlHelper.ConvertFromString<T>(firstChildNodeByName.InnerText);
        return obj;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}] in [{1}]", (object) childNodeName, (object) parentNode.Name);
        throw;
      }
    }

    public static string GetChildNodeText(XmlNode parentNode, string childNodeName)
    {
      string childNodeText = CXmlHelper.GetChildNodeText<string>(parentNode, childNodeName);
      if (!CXmlHelper.UseStringInterning)
        return childNodeText;
      return string.Intern(childNodeText);
    }

    public static T GetChildNodeText<T>(XmlNode parentNode, string childNodeName)
    {
      XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName);
      if (firstChildNodeByName == null)
        throw new CNotFoundException("Xml node is not found. Name: [{0}]", new object[1]
        {
          (object) childNodeName
        });
      return CXmlHelper.ConvertFromString<T>(firstChildNodeByName.InnerText);
    }

    public static DateTime? GetNullableDateTime(
      XmlNode parentNode,
      string childNodeName,
      IFormatProvider provider,
      DateTimeKind kind)
    {
      DateTime? nullableDateTime = CXmlHelper.GetNullableDateTime(parentNode, childNodeName, provider);
      if (nullableDateTime.HasValue)
        return new DateTime?(DateTime.SpecifyKind(nullableDateTime.Value, kind));
      return new DateTime?();
    }

    public static DateTime? GetNullableDateTime(
      XmlNode parentNode,
      string childNodeName,
      IFormatProvider provider)
    {
      try
      {
        DateTime? nullable = new DateTime?();
        string childNodeValue = CXmlHelper.GetChildNodeValue(parentNode, childNodeName, (string) null);
        if (!string.IsNullOrEmpty(childNodeValue))
          nullable = new DateTime?(DateTime.Parse(childNodeValue, provider));
        return nullable;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}] in [{1}]", (object) childNodeName, (object) parentNode.Name);
        throw;
      }
    }

    public static Guid? GetNullableGuid(XmlNode parentNode, string childNodeName)
    {
      try
      {
        Guid? nullable = new Guid?();
        string childNodeValue = CXmlHelper.GetChildNodeValue(parentNode, childNodeName, (string) null);
        if (childNodeValue != null)
          nullable = new Guid?(Guid.Parse(childNodeValue));
        return nullable;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}] in [{1}]", (object) childNodeName, (object) parentNode.Name);
        throw;
      }
    }

    public static T GetNodeText<T>(XmlNode node)
    {
      try
      {
        return CXmlHelper.ConvertFromString<T>(node.InnerText);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}]", (object) node.Name);
        throw;
      }
    }

    public static string GetNodeText(XmlNode node)
    {
      try
      {
        string innerText = node.InnerText;
        return CXmlHelper.UseStringInterning ? string.Intern(innerText) : innerText;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Error while parsing node [{0}]", (object) node.Name);
        throw;
      }
    }

    public static void SetChildNodeValue<T>(
      XmlNode parentNode,
      string childNodeName,
      T childNodeValue)
    {
      CXmlHelper.SetChildNodeValue<T>(parentNode, childNodeName, childNodeValue, false);
    }

    public static void SetChildNodeValue<T>(
      XmlNode parentNode,
      string childNodeName,
      T childNodeValue,
      bool codeText)
    {
      XmlNode xmlNode = CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName) ?? (XmlNode) CXmlHelper.AddElement(parentNode, childNodeName);
      string text = string.Empty;
      if ((object) childNodeValue != null)
        text = Convert.ToString((object) childNodeValue, (IFormatProvider) CultureInfo.InvariantCulture);
      if (codeText)
        text = CXmlHelper.CodeText(text);
      xmlNode.InnerXml = text;
    }

    public static XmlNode SetChildNode(XmlNode parentNode, string nodeName)
    {
      return CXmlHelper.FindFirstChildNodeByName(parentNode, nodeName) ?? (XmlNode) CXmlHelper.AddElement(parentNode, nodeName);
    }

    public static XmlNode GetOrAddChildNode(
      XmlNode parentNode,
      string nodeName,
      bool recreateNode)
    {
      XmlNode oldChild = CXmlHelper.FindFirstChildNodeByName(parentNode, nodeName);
      if (oldChild != null && recreateNode)
      {
        parentNode.RemoveChild(oldChild);
        oldChild = (XmlNode) null;
      }
      if (oldChild == null)
        oldChild = (XmlNode) CXmlHelper.AddElement(parentNode, nodeName);
      return oldChild;
    }

    private static T ConvertFromString<T>(string strValue)
    {
      System.Type type1 = Nullable.GetUnderlyingType(typeof (T));
      if ((object) type1 == null)
        type1 = typeof (T);
      System.Type type2 = type1;
      if (type2 == typeof(Guid))
          return (T) Convert.ChangeType(strValue, type2);
      //  return (T) (ValueType) new Guid(strValue);
      if (type2.IsEnum)
        return (T) Enum.Parse(type2, strValue);
      if (type2 == typeof (TimeSpan))
          return (T) Convert.ChangeType(strValue, type2);
       // return (T) (ValueType) TimeSpan.Parse(strValue);
      if (!(type2 == typeof (DateTime)))
        return (T) Convert.ChangeType((object) strValue, type2);
      DateTime result = DateTime.MinValue;
      if (!DateTime.TryParse(strValue, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        result = DateTime.Parse(strValue);
      return (T) Convert.ChangeType(strValue, type2);
     // return (T) (ValueType) result;
    }

    public static T GetAttrValueEx<T>(XmlNode node, string attrName)
    {
      try
      {
        return CXmlHelper.ConvertFromString<T>(node.Attributes[attrName].Value);
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute \"{0}\" is not defined.", (object) attrName);
        throw;
      }
    }

    private static T GetAttrValue_<T>(XmlAttribute attr)
    {
      if (typeof (T) == typeof (Guid))
          return (T) Convert.ChangeType(attr.Value, typeof(T));
       // return (T) (ValueType) new Guid(attr.Value);
      if (typeof (T).IsEnum)
        return (T) Enum.Parse(typeof (T), attr.Value);
      return (T) Convert.ChangeType((object) attr.Value, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static T GetAttrValue<T>(XmlNode node, string attrName, T defaultValue)
    {
      XmlAttribute attribute = node.Attributes[attrName];
      if (attribute == null)
        return defaultValue;
      return CXmlHelper.GetAttrValue_<T>(attribute);
    }

    public static T GetNodeValue<T>(XmlNode node)
    {
      return CXmlHelper.ConvertFromString<T>(node.Value);
    }

    public static void SetNodeValue<T>(XmlNode parentNode, string nodeName, T nodeValue)
    {
      XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, nodeName);
      if (firstChildNodeByName != null)
        firstChildNodeByName.InnerXml = nodeValue.ToString();
      else
        CXmlHelper.AddChildNode(parentNode, nodeName, nodeValue.ToString());
    }

    public static XmlNode FindFirstNodeByName(XmlDocument doc, string nodeName)
    {
      IEnumerator enumerator = doc.GetElementsByTagName(nodeName).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
          return (XmlNode) enumerator.Current;
      }
      finally
      {
        (enumerator as IDisposable)?.Dispose();
      }
      return (XmlNode) null;
    }

    public static XmlNode GetFirstNodeByName(XmlDocument doc, string nodeName)
    {
      XmlNode firstNodeByName = CXmlHelper.FindFirstNodeByName(doc, nodeName);
      if (firstNodeByName == null)
        throw new CAppException("Node [{0}] does not exist.", new object[1]
        {
          (object) nodeName
        });
      return firstNodeByName;
    }

    public static XmlNode[] FindChildNodesByName(XmlNode parentNode, string nodeName)
    {
      List<XmlNode> xmlNodeList = new List<XmlNode>();
      foreach (XmlNode childNode in parentNode.ChildNodes)
      {
        if (childNode.Name == nodeName)
          xmlNodeList.Add(childNode);
      }
      return xmlNodeList.ToArray();
    }

    public static XmlNode FindFirstChildNodeByName(XmlNode parentNode, string nodeName)
    {
      if (!parentNode.HasChildNodes)
        return (XmlNode) null;
      foreach (XmlNode childNode in parentNode.ChildNodes)
      {
        if (childNode.Name == nodeName)
          return childNode;
      }
      return (XmlNode) null;
    }

    public static XmlNode FindFirstRenamedChildNodeByName(
      XmlNode parentNode,
      string oldNodeName,
      string newNodeName)
    {
      return CXmlHelper.FindFirstChildNodeByName(parentNode, newNodeName) ?? CXmlHelper.FindFirstChildNodeByName(parentNode, oldNodeName);
    }

    public static XmlNode GetFirstChildNodeByName(XmlNode parentNode, string nodeName)
    {
      XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, nodeName);
      if (firstChildNodeByName == null)
        throw new CAppException("Node \"{0}\" does not exist.", new object[1]
        {
          (object) nodeName
        });
      return firstChildNodeByName;
    }

    public static string CodeText(string text)
    {
      StringBuilder stringBuilder = new StringBuilder(text);
      stringBuilder.Replace("&", "&amp;");
      stringBuilder.Replace("<", "&lt;");
      stringBuilder.Replace("'", "&#39;");
      stringBuilder.Replace("\"", "&#34;");
      stringBuilder.Replace(">", "&gt;");
      return stringBuilder.ToString();
    }

    public static string EscapeInvalidXmlCharacters(string input)
    {
      if (input.All<char>(new Func<char, bool>(XmlConvert.IsXmlChar)))
        return input;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in input)
      {
        if (XmlConvert.IsXmlChar(ch))
          stringBuilder.Append(ch);
        else
          stringBuilder.Append("&#" + (object) (byte) ch + ";");
      }
      return stringBuilder.ToString();
    }

    public static XmlNode ReplaceNode(XmlNode oldNode, XmlNode newNode)
    {
      XmlNode parentNode = oldNode.ParentNode;
      XmlNode newChild = parentNode.OwnerDocument.ImportNode(newNode, true);
      parentNode.ReplaceChild(newChild, oldNode);
      return newChild;
    }

    public static void RemoveChildNode(XmlNode parentNode, string childNodeName)
    {
      XmlNode firstChildNodeByName = CXmlHelper.FindFirstChildNodeByName(parentNode, childNodeName);
      if (firstChildNodeByName == null)
        return;
      parentNode.RemoveChild(firstChildNodeByName);
    }

    public static void RemoveAllChildNode(XmlNode parentNode, string childNodeName)
    {
      XmlNode[] childNodesByName = CXmlHelper.FindChildNodesByName(parentNode, childNodeName);
      if (((IReadOnlyCollection<XmlNode>) childNodesByName).IsNullOrEmpty<XmlNode>())
        return;
      foreach (XmlNode oldChild in childNodesByName)
        parentNode.RemoveChild(oldChild);
    }

    public static string DecodeText(string encodedText)
    {
      StringBuilder stringBuilder = new StringBuilder(encodedText);
      stringBuilder.Replace("&lt;", "<");
      stringBuilder.Replace("&#39;", "'");
      stringBuilder.Replace("&#34;", "\"");
      stringBuilder.Replace("&gt;", ">");
      stringBuilder.Replace("&amp;", "&");
      return stringBuilder.ToString();
    }

    public static string DecodeUntilOriginal(string path)
    {
      string encodedText = path;
      for (string str = string.Empty; encodedText != str; encodedText = CXmlHelper.DecodeText(encodedText))
        str = encodedText;
      return encodedText;
    }

    public static string PackCData(string xml)
    {
      return "<![CDATA[" + xml + "]]>";
    }

    public static string UnPackCData(string cdata)
    {
      if (!cdata.StartsWith("<![CDATA[") || !cdata.EndsWith("]]>"))
        throw new CNotFoundException("CData is not found. Xml: [{0}]", new object[1]
        {
          (object) cdata
        });
      cdata = cdata.Remove(0, "<![CDATA[".Length);
      cdata = cdata.Remove(cdata.Length - "]]>".Length, "]]>".Length);
      return cdata;
    }

    public static XmlDocument DocFromStr(string xml)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xml);
      return xmlDocument;
    }

    public static XmlDocument DocFromStrSafe(string xml)
    {
      try
      {
        return CXmlHelper.DocFromStr(xml);
      }
      catch
      {
        return (XmlDocument) null;
      }
    }

    public static void RemoveAllChildNodes(XmlNode node)
    {
      foreach (XmlNode oldChild in node.ChildNodes.Cast<XmlNode>().ToList<XmlNode>())
        node.RemoveChild(oldChild);
    }

    public static string BoolToStr(bool bVal)
    {
      return bVal ? "true" : "false";
    }

    public static bool StrToBool(string val)
    {
      try
      {
        return string.Compare(val, "true", true) == 0 || string.Compare(val, "false", true) != 0 && string.Compare(val, "0", true) != 0;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Value of the attribute is not a boolean ([{0}]).", (object) val);
        throw;
      }
    }
  }
}
