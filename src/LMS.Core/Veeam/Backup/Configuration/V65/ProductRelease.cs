using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public class ProductRelease : IProductRelease, IComparable, IComparable<ProductRelease>
  {
    private readonly IDatabaseVersion _databaseVersion;

    public string Name { get; private set; }

    public Version Version { get; private set; }

    public bool IsSupported { get; private set; }

    public bool IsCurrent { get; private set; }

    public int UpdateNumber { get; private set; }

    public ProductRelease(
      string name,
      Version assemblyVersion,
      int updateNumber,
      IDatabaseVersion databaseVersion,
      bool isSupported,
      bool isCurrent)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (assemblyVersion == (Version) null)
        throw new ArgumentNullException(nameof (assemblyVersion));
      if (databaseVersion == null)
        throw new ArgumentNullException(nameof (databaseVersion));
      this._databaseVersion = databaseVersion;
      this.Name = name;
      this.Version = assemblyVersion;
      this.IsSupported = isSupported;
      this.IsCurrent = isCurrent;
      this.UpdateNumber = updateNumber;
    }

    public int this[DatabaseVersionType type]
    {
      get
      {
        return this._databaseVersion[type];
      }
    }

    public int CompareTo(object obj)
    {
      return this.CompareTo(obj as ProductRelease);
    }

    public int CompareTo(ProductRelease other)
    {
      if (other == null)
        return 1;
      return ((IEnumerable<Func<int>>) new Func<int>[4]
      {
        (Func<int>) (() => string.Compare(this.Name, other.Name, StringComparison.Ordinal)),
        (Func<int>) (() => this.Version.CompareTo(other.Version)),
        (Func<int>) (() => this.IsSupported.CompareTo(other.IsSupported)),
        (Func<int>) (() => this.UpdateNumber.CompareTo(other.UpdateNumber))
      }).Select<Func<int>, int>((Func<Func<int>, int>) (comparable => comparable())).FirstOrDefault<int>((Func<int, bool>) (result => result != 0));
    }

    public override string ToString()
    {
      return this.Version.ToString();
    }
  }
}
