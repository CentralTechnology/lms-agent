using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  [COptionsScope("CloudConnect")]
  public sealed class CCloudConnectOptionsScope
  {
    private static readonly Guid VBRProductId = new Guid("{B1E61D9B-8D78-4419-8F63-D21279F71A56}");
    private static readonly Guid VAWProductId = new Guid("{2C9E3B09-F27C-4E27-8A8E-CFEF030041F2}");
    private static readonly Guid VALProductId = new Guid("{819A7952-2B80-4287-A99A-79DD0578E5AD}");
    [COptionsValue]
    public readonly int TcpKeepAliveTimeoutMsec = 10000;
    [COptionsValue]
    public readonly int TcpKeepAliveIntervalMsec = 60000;
    [COptionsValue]
    public readonly int MinimalSupportedProtocolVersion = 2;
    [COptionsValue]
    public readonly int MaximalSupportedProtocolVersion = 6;
    [COptionsValue]
    public readonly int CurrentSupportedProtocolVersion = 6;
    [COptionsValue]
    public readonly bool IsSubtenantConnectionEnabled;
    [COptionsValue]
    public readonly Version MinimalSupportedVBRVersion;
    [COptionsValue]
    public readonly Version MinimalSupportedVAWVersion;
    [COptionsValue]
    public readonly Version MinimalSupportedVALVersion;

    public CCloudConnectOptionsScope(IOptionsReader reader)
    {
      SOptionsLoader.LoadOptions<CCloudConnectOptionsScope>(this, reader);
    }

    public bool IsMinimalSupportedProductVersionSpecified
    {
      get
      {
        Version version = this.MinimalSupportedVBRVersion;
        if ((object) version == null)
        {
          Version supportedVawVersion = this.MinimalSupportedVAWVersion;
          version = (object) supportedVawVersion != null ? supportedVawVersion : this.MinimalSupportedVALVersion;
        }
        return version != (Version) null;
      }
    }

    public Version FindMinimalSupportedProductVersion(Guid? productId)
    {
      Guid? nullable1 = productId;
      Guid vbrProductId = CCloudConnectOptionsScope.VBRProductId;
      if ((!nullable1.HasValue ? 0 : (nullable1.GetValueOrDefault() == vbrProductId ? 1 : 0)) != 0)
        return this.MinimalSupportedVBRVersion;
      Guid? nullable2 = productId;
      Guid vawProductId = CCloudConnectOptionsScope.VAWProductId;
      if ((!nullable2.HasValue ? 0 : (nullable2.GetValueOrDefault() == vawProductId ? 1 : 0)) != 0)
        return this.MinimalSupportedVAWVersion;
      Guid? nullable3 = productId;
      Guid valProductId = CCloudConnectOptionsScope.VALProductId;
      if ((!nullable3.HasValue ? 0 : (nullable3.GetValueOrDefault() == valProductId ? 1 : 0)) != 0)
        return this.MinimalSupportedVALVersion;
      return (Version) null;
    }
  }
}
