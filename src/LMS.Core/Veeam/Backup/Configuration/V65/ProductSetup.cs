using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductSetup : IProductSetup, IComparable, IComparable<ProductSetup>
  {
    public Version Version { get; private set; }

    public ProcessPlatform Platform { get; private set; }

    public string FileName { get; private set; }

    public Guid PackageCode { get; private set; }

    public Guid ProductCode { get; private set; }

    public Guid UpgradeCode { get; private set; }

    public bool IsCurrent { get; private set; }

    public ProductSetup(
      Version setupVersion,
      ProcessPlatform platform,
      string fileName,
      Guid packageCode,
      Guid productCode,
      Guid upgradeCode,
      bool isCurrent)
    {
      if (setupVersion == (Version) null)
        throw new ArgumentNullException(nameof (setupVersion));
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentNullException(nameof (fileName));
      this.Version = setupVersion;
      this.Platform = platform;
      this.FileName = fileName;
      this.PackageCode = packageCode;
      this.ProductCode = productCode;
      this.UpgradeCode = upgradeCode;
      this.IsCurrent = isCurrent;
    }

    public int CompareTo(object obj)
    {
      return this.CompareTo(obj as ProductSetup);
    }

    public int CompareTo(ProductSetup other)
    {
      if (other == null)
        return 1;
      return ((IEnumerable<Func<int>>) new Func<int>[6]
      {
        (Func<int>) (() => this.Version.CompareTo(other.Version)),
        (Func<int>) (() => this.Platform.CompareTo((object) other.Platform)),
        (Func<int>) (() => this.PackageCode.CompareTo(other.PackageCode)),
        (Func<int>) (() => this.ProductCode.CompareTo(other.ProductCode)),
        (Func<int>) (() => this.UpgradeCode.CompareTo(other.UpgradeCode)),
        (Func<int>) (() => this.IsCurrent.CompareTo(other.IsCurrent))
      }).Select<Func<int>, int>((Func<Func<int>, int>) (comparable => comparable())).FirstOrDefault<int>((Func<int, bool>) (result => result != 0));
    }

    public override string ToString()
    {
      return this.Version.ToString();
    }
  }
}
