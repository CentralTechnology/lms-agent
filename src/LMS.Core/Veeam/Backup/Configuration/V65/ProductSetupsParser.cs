using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductSetupsParser
  {
    private readonly XmlDocument _document;
    private readonly XmlNamespaceManager _manager;

    public ProductSetupsParser(XmlDocument document)
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

    public IEnumerable<IProductSetup> Parse()
    {
      IEnumerator enumerator = this._document.DocumentElement.SelectNodes(ProductSetupsParser.Xml.SelectNodes, this._manager).GetEnumerator();
      try
      {
        while (enumerator.MoveNext())
        {
          XmlNode node = (XmlNode) enumerator.Current;
          string versionText = node.Attributes["version"].Value;
          string platformText = node.Attributes["platform"].Value;
          string fileNameText = node.Attributes["filename"].Value;
          string packageCodeText = node.Attributes["packagecode"].Value;
          string productCodeText = node.Attributes["productcode"].Value;
          string upgradeCodeText = node.Attributes["upgradecode"].Value;
          Version setupVersion = Version.Parse(versionText);
          ProcessPlatform productPlatform = (ProcessPlatform) Enum.Parse(typeof (ProcessPlatform), platformText, true);
          Guid packageCode = Guid.Parse(packageCodeText);
          Guid productCode = Guid.Parse(productCodeText);
          Guid upgradeCode = Guid.Parse(upgradeCodeText);
          bool isCurrent = setupVersion.IsEmpty() && packageCode == Guid.Empty;
          yield return (IProductSetup) new ProductSetup(setupVersion, productPlatform, fileNameText, packageCode, productCode, upgradeCode, isCurrent);
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
      public static readonly string SelectNodes = string.Format("//{0}:product/{0}:setup", (object) "ns");
      public const string NamespacePrefix = "ns";
      public const string VersionAttribute = "version";
      public const string PlatformAttribute = "platform";
      public const string FileNameAttribute = "filename";
      public const string PackageCodeAttribute = "packagecode";
      public const string ProductCodeAttribute = "productcode";
      public const string UpgradeCodeAttribute = "upgradecode";
    }
  }
}
