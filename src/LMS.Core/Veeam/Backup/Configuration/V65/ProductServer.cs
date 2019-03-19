using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductServer : IProductServer, IComparable<ProductServer>
  {
    public ServerFamily Family { get; private set; }

    public int ServicePack { get; private set; }

    public Version Version { get; private set; }

    public int CompatibilityLevel { get; private set; }

    private ProductServer(ServerFamily family, int servicePack, Version version)
    {
      if (servicePack < 0)
        throw new ArgumentOutOfRangeException(nameof (servicePack));
      if (version == (Version) null)
        throw new ArgumentNullException(nameof (version));
      this.Family = family;
      this.ServicePack = servicePack;
      this.Version = version;
      this.CompatibilityLevel = version.Major * 10;
    }

    public string Base
    {
      get
      {
        ProductServerFamilyName serverFamilyName = new ProductServerFamilyName(this.Family);
        if (!string.IsNullOrEmpty((string) serverFamilyName))
          return (string) serverFamilyName;
        return string.Empty;
      }
    }

    public string Name
    {
      get
      {
        if (string.IsNullOrEmpty(this.Base))
          return string.Empty;
        if (this.ServicePack != 0)
          return string.Format("{0} Service Pack {1}", (object) this.Base, (object) this.ServicePack);
        return this.Base;
      }
    }

    public int CompareTo(object obj)
    {
      return this.CompareTo(obj as ProductServer);
    }

    public int CompareTo(ProductServer other)
    {
      if (other == null)
        return 1;
      return this.Version.CompareTo(other.Version);
    }

    public string ToString(string format)
    {
      switch (format)
      {
        case "B":
          return this.Base;
        case "S":
          return this.Name;
        case "F":
          return string.Format("{0} ({1})", (object) this.Name, (object) this.Version);
        default:
          throw new ArgumentOutOfRangeException(nameof (format));
      }
    }

    public override string ToString()
    {
      return this.ToString("F");
    }

    public static IProductServer Create(
      ServerFamily family,
      int servicePack,
      Version version)
    {
      return (IProductServer) new ProductServer(family, servicePack, version);
    }

    private static class Resources
    {
      public const string ServicePackFormat = "{0} Service Pack {1}";
    }

    private static class Formats
    {
      public const string Base = "B";
      public const string Short = "S";
      public const string Full = "F";
    }
  }
}
