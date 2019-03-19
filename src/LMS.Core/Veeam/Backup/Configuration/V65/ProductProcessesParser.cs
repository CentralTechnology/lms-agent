using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductProcessesParser
  {
    private readonly XmlDocument _document;
    private readonly XmlNamespaceManager _manager;

    public ProductProcessesParser(XmlDocument document)
    {
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      if (document.Schemas.Count == 0)
        throw new ArgumentNullException(nameof (document));
      if (document.DocumentElement == null)
        throw new ArgumentNullException(nameof (document));
      this._document = document;
      this._manager = XmlNamespaceCreator.Create(this._document, "ns");
    }

    private static IEnumerable<ProductProcessOwner> ParseOwners(
      IEnumerable nodeList)
    {
      if (nodeList == null)
        throw new ArgumentNullException(nameof (nodeList));
      IEnumerator enumerator = nodeList.GetEnumerator();
      try
      {
        while (enumerator.MoveNext())
        {
          XmlNode node = (XmlNode) enumerator.Current;
          string idText = node.Attributes["id"].Value;
          string platformText = node.Attributes["platform"].Value;
          Guid id = Guid.Parse(idText);
          ProcessPlatform platform = (ProcessPlatform) Enum.Parse(typeof (ProcessPlatform), platformText, true);
          yield return new ProductProcessOwner(id, platform);
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        disposable?.Dispose();
      }
    }

    public IEnumerable<IProductProcess> Parse()
    {
      IEnumerator enumerator = this._document.DocumentElement.SelectNodes(ProductProcessesParser.Xml.SelectNodes, this._manager).GetEnumerator();
      try
      {
        while (enumerator.MoveNext())
        {
          XmlNode node = (XmlNode) enumerator.Current;
          string idText = node.Attributes["id"].Value;
          string platformText = node.Attributes["platform"].Value;
          string typeText = node.Attributes["type"].Value;
          string nameText = node.Attributes["name"].Value;
          string fileNameText = node.Attributes["filename"].Value;
          XmlAttribute descriptionAttr = node.Attributes["description"];
          string description = descriptionAttr == null ? string.Empty : descriptionAttr.Value;
          Guid id = Guid.Parse(idText);
          ProcessPlatform platform = (ProcessPlatform) Enum.Parse(typeof (ProcessPlatform), platformText, true);
          ProcessType type = (ProcessType) Enum.Parse(typeof (ProcessType), typeText, true);
          XmlNode install = node.SelectSingleNode(ProductProcessesParser.Xml.SelectInstallNodes, this._manager);
          string productCode = install == null ? string.Empty : install.Attributes["productcode"].Value;
          string componentId = install == null ? string.Empty : install.Attributes["componentid"].Value;
          if (StringComparer.OrdinalIgnoreCase.Compare(node.Name, "process") == 0)
          {
            ProductProcessOwner[] owners = ProductProcessesParser.ParseOwners((IEnumerable) node.SelectNodes(ProductProcessesParser.Xml.SelectOwnerNodes, this._manager)).ToArray<ProductProcessOwner>();
            yield return (IProductProcess) new ProductProcess(id, platform, type, nameText, fileNameText, owners, productCode, componentId, description);
          }
          else if (StringComparer.OrdinalIgnoreCase.Compare(node.Name, "service") == 0)
          {
            string orderText = node.Attributes["order"].Value;
            string serviceNameText = node.Attributes["servicename"].Value;
            string canBeDisabledText = node.Attributes["canbedisabled"].Value;
            int order = int.Parse(orderText);
            bool canBeDisabled = bool.Parse(canBeDisabledText);
            yield return (IProductProcess) new ProductService(id, platform, type, nameText, fileNameText, order, serviceNameText, canBeDisabled, productCode, componentId, description);
          }
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        disposable?.Dispose();
      }
    }

    private static class Xml
    {
      public static readonly string SelectNodes = string.Format("//{0}:product/{0}:{1} | //{0}:product/{0}:{2}", (object) "ns", (object) "process", (object) "service");
      public static readonly string SelectOwnerNodes = string.Format("{0}:{1}", (object) "ns", (object) "owner");
      public static readonly string SelectInstallNodes = string.Format("{0}:{1}", (object) "ns", (object) "install");
      public const string NamespacePrefix = "ns";
      public const string IdAttribute = "id";
      public const string PlatformAttribute = "platform";
      public const string TypeAttribute = "type";
      public const string NameAttribute = "name";
      public const string FileNameAttribute = "filename";
      public const string ServiceNameAttribute = "servicename";
      public const string ServiceDisabledAttribute = "canbedisabled";
      public const string OrderAttribute = "order";
      public const string ProcessNode = "process";
      public const string ServiceNode = "service";
      private const string OwnerNode = "owner";
      private const string InstallNode = "install";
      public const string ProductCode = "productcode";
      public const string ComponentId = "componentid";
      public const string Description = "description";
    }
  }
}
