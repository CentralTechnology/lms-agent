using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LMS.Core.Common;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class XmlNodeExm
  {
    public static IEnumerable<T> NullSafeEnumerateChilds<T>(
      this XmlNode node,
      string childName)
      where T : XmlNode
    {
      Exceptions.CheckArgumentNullOrEmptyException(childName, nameof (childName));
      if (node == null)
        return (IEnumerable<T>) new T[0];
      return node.ChildNodes.OfType<T>().Where<T>((Func<T, bool>) (el => el.Name == childName));
    }

    public static XmlDocument GetXmlDocument(this XmlNode node)
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      XmlDocument xmlDocument = node as XmlDocument ?? node.OwnerDocument;
      if (xmlDocument == null)
        throw ExceptionFactory.XmlException("Node '{0}' has not parent XmlDocument.");
      return xmlDocument;
    }

    public static XmlElement CreateElement(this XmlNode node, string elementName)
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      Exceptions.CheckArgumentNullOrEmptyException(elementName, nameof (elementName));
      return node.GetXmlDocument().CreateElement(elementName);
    }

    public static T AppendNode<T>(this XmlNode node, T child) where T : XmlNode
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      Exceptions.CheckArgumentNullException<T>(child, nameof (child));
      return (T) node.AppendChild((XmlNode) child);
    }

    public static XmlElement EnsureElementExists(this XmlNode node, string elementName)
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      Exceptions.CheckArgumentNullOrEmptyException(elementName, nameof (elementName));
      XmlElement xmlElement = node[elementName];
      if (xmlElement != null)
        return xmlElement;
      XmlElement element = node.CreateElement(elementName);
      return node.AppendNode<XmlElement>(element);
    }

    public static T ImportNode<T>(this XmlNode node, T child) where T : XmlNode
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      Exceptions.CheckArgumentNullException<T>(child, nameof (child));
      return (T) node.GetXmlDocument().ImportNode((XmlNode) child, true);
    }

    public static T ImportNode<T>(this XmlNode node, T child, string newName) where T : XmlNode
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      Exceptions.CheckArgumentNullException<T>(child, nameof (child));
      Exceptions.CheckArgumentNullOrEmptyException(newName, nameof (newName));
      XmlDocument xmlDocument = node.GetXmlDocument();
      T obj = (T) xmlDocument.ImportNode((XmlNode) child, true);
      XmlAttribute[] xmlAttributeArray = obj.Attributes == null ? (XmlAttribute[]) null : obj.Attributes.OfType<XmlAttribute>().ToArray<XmlAttribute>();
      XmlNode[] array = obj.ChildNodes.OfType<XmlNode>().ToArray<XmlNode>();
      T node1 = (T) xmlDocument.CreateNode(obj.NodeType, newName, obj.NamespaceURI);
      if (xmlAttributeArray != null)
      {
        foreach (XmlAttribute node2 in xmlAttributeArray)
          node1.Attributes.Append(node2);
      }
      foreach (XmlNode newChild in array)
        node1.AppendChild(newChild);
      return node1;
    }

    public static XmlElement UpdateElement(this XmlNode node, XmlElement child)
    {
      Exceptions.CheckArgumentNullException<XmlNode>(node, nameof (node));
      Exceptions.CheckArgumentNullException<XmlElement>(child, nameof (child));
      XmlElement xmlElement = node[child.Name];
      if (xmlElement != null)
        node.RemoveChild((XmlNode) xmlElement);
      XmlElement child1 = node.ImportNode<XmlElement>(child);
      return node.AppendNode<XmlElement>(child1);
    }
  }
}
