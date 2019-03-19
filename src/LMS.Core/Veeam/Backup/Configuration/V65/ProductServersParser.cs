using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductServersParser
  {
    private readonly XmlDocument _document;
    private readonly XmlNamespaceManager _manager;

    public ProductServersParser(XmlDocument document)
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

    public void Parse()
    {
      List<IProductServer> productServerList = new List<IProductServer>();
      if (this._document.DocumentElement.HasAttribute("minsupportedversion"))
      {
        Version version = new Version(this._document.DocumentElement.Attributes["minsupportedversion"].Value);
        this.MinSupportedVersion = version.IsEmpty() ? (Version) null : version;
      }
      foreach (XmlNode selectNode in this._document.DocumentElement.SelectNodes(ProductServersParser.Xml.SelectNodes, this._manager))
      {
        string str = selectNode.Attributes["family"].Value;
        string input = selectNode.Attributes["version"].Value;
        ServerFamily family = (ServerFamily) Enum.Parse(typeof (ServerFamily), str, true);
        Version version = Version.Parse(input);
        if (selectNode.Attributes["servicepack"] == null)
        {
          productServerList.Add(ProductServer.Create(family, 0, version));
        }
        else
        {
          int servicePack = int.Parse(selectNode.Attributes["servicepack"].Value);
          productServerList.Add(ProductServer.Create(family, servicePack, version));
        }
      }
      this.Servers = (IEnumerable<IProductServer>) productServerList;
    }

    public Version MinSupportedVersion { get; private set; }

    public IEnumerable<IProductServer> Servers { get; private set; }

    private static class Xml
    {
      public static readonly string SelectNodes = string.Format("//{0}:product/{0}:server", (object) "ns");
      public const string NamespacePrefix = "ns";
      public const string FamilyAttribute = "family";
      public const string ServicePackAttribute = "servicepack";
      public const string VersionAttribute = "version";
      public const string MinSupportedVersionAttribute = "minsupportedversion";
    }
  }
}
