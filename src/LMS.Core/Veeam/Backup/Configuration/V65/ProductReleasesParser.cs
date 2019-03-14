using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductReleasesParser
  {
    private readonly XmlDocument _document;
    private readonly XmlNamespaceManager _manager;
    private readonly IList<IProductRelease> _releases;

    public Version Upgdarable { get; private set; }

    public bool Preview { get; private set; }

    public ProductReleasesParser(XmlDocument document)
    {
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      if (document.Schemas.Count == 0)
        throw new ArgumentNullException(nameof (document));
      if (document.DocumentElement == null)
        throw new ArgumentNullException(nameof (document));
      this._document = document;
      this._manager = XmlNamespaceCreator.Create(this._document, "ns");
      this._releases = (IList<IProductRelease>) new List<IProductRelease>();
    }

    public IEnumerable<IProductRelease> Releases
    {
      get
      {
        return (IEnumerable<IProductRelease>) this._releases;
      }
    }

    public void Parse()
    {
      this._releases.Clear();
      string input1 = this._document.DocumentElement.Attributes["upgradable"].Value;
      string str1 = this._document.DocumentElement.Attributes["preview"].Value;
      this.Upgdarable = Version.Parse(input1);
      this.Preview = bool.Parse(str1);
      foreach (XmlNode selectNode in this._document.DocumentElement.SelectNodes(ProductReleasesParser.Xml.SelectNodes, this._manager))
      {
        string name = selectNode.Attributes["name"].Value;
        string input2 = selectNode.Attributes["version"].Value;
        string str2 = selectNode.Attributes["supported"].Value;
        Version version = Version.Parse(input2);
        bool isSupported = bool.Parse(str2);
        int updateNumber = selectNode.Attributes["update"] != null ? int.Parse(selectNode.Attributes["update"].Value) : -1;
        if (selectNode.Attributes["database"] == null)
        {
          IDatabaseVersion databaseVersion = ProductDatabaseVersion.Create(0, 0);
          bool isCurrent = version.IsEmpty();
          this._releases.Add((IProductRelease) new ProductRelease(name, version, updateNumber, databaseVersion, isSupported, isCurrent));
        }
        else
        {
          IDatabaseVersion databaseVersion = DatabaseVersionParser.Parse(selectNode.Attributes["database"].Value);
          bool isCurrent = version.IsEmpty() && databaseVersion[DatabaseVersionType.Schema] == 0 && databaseVersion[DatabaseVersionType.Content] == 0;
          this._releases.Add((IProductRelease) new ProductRelease(name, version, updateNumber, databaseVersion, isSupported, isCurrent));
        }
      }
    }

    private static class Xml
    {
      public static readonly string SelectNodes = string.Format("//{0}:product/{0}:release", (object) "ns");
      public const string NamespacePrefix = "ns";
      public const string UpgradableAttribute = "upgradable";
      public const string PreviewAttribute = "preview";
      public const string VersionAttribute = "version";
      public const string DatabaseAttribute = "database";
      public const string SupportedAttribute = "supported";
      public const string NameAttribute = "name";
      public const string UpdateAttribute = "update";
    }
  }
}
