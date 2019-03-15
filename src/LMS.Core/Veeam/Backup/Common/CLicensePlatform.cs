using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LMS.Core.Common;

namespace LMS.Core.Veeam.Backup.Common
{
  [Serializable]
  public class CLicensePlatform : IEquatable<CLicensePlatform>, IComparable<CLicensePlatform>, IXmlSerializable
  {
      public static readonly CLicensePlatform Vmware = SLicensePlatformController.Register(ELicensePlatform.VmWare);
      public static readonly CLicensePlatform HyperV = SLicensePlatformController.Register(ELicensePlatform.HyperV);
    private const string PlatformTag = "Platform";
    private const string ModeTag = "Mode";

    public ELicensePlatform Platform { get; set; }

    public EEpLicenseMode? LicenseMode { get; set; }

    protected CLicensePlatform()
    {
    }

    internal CLicensePlatform(ELicensePlatform platform, EEpLicenseMode licenseMode)
      : this(platform)
    {
      this.LicenseMode = new EEpLicenseMode?(licenseMode);
    }

    internal CLicensePlatform(ELicensePlatform platform)
    {
      this.Platform = platform;
    }

    public EEpLicenseMode GetLicenseMode()
    {
      if (!this.LicenseMode.HasValue)
        throw ExceptionFactory.Create("LicenseMode is not specified. Platform: {0}", (object) this.Platform);
      return this.LicenseMode.Value;
    }

    public bool Equals(CLicensePlatform other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (this.Platform != other.Platform)
        return false;
      EEpLicenseMode? licenseMode1 = this.LicenseMode;
      EEpLicenseMode? licenseMode2 = other.LicenseMode;
      if (licenseMode1.GetValueOrDefault() == licenseMode2.GetValueOrDefault())
        return licenseMode1.HasValue == licenseMode2.HasValue;
      return false;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return this.Equals((CLicensePlatform) obj);
    }

    public static bool operator ==(CLicensePlatform left, CLicensePlatform right)
    {
      return object.Equals((object) left, (object) right);
    }

    public static bool operator !=(CLicensePlatform left, CLicensePlatform right)
    {
      return !object.Equals((object) left, (object) right);
    }

    public override int GetHashCode()
    {
      return (int) this.Platform * 397 ^ this.LicenseMode.GetHashCode();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[ " + (object) this.Platform + " ");
      if (this.LicenseMode.HasValue)
        stringBuilder.Append("| " + (object) this.LicenseMode + " ");
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }

    public int CompareTo(CLicensePlatform other)
    {
      if (object.ReferenceEquals((object) this, (object) other))
        return 0;
      if (object.ReferenceEquals((object) null, (object) other))
        return 1;
      int num = this.Platform.CompareTo((object) other.Platform);
      if (num != 0)
        return num;
      return Nullable.Compare<EEpLicenseMode>(this.LicenseMode, other.LicenseMode);
    }

    public XmlSchema GetSchema()
    {
      return (XmlSchema) null;
    }

    public void ReadXml(XmlReader reader)
    {
      this.Platform = (ELicensePlatform) reader.GetAttribute<int>("Platform");
      int? attribute = reader.FindAttribute<int>("Mode");
      this.LicenseMode = attribute.HasValue ? new EEpLicenseMode?((EEpLicenseMode) attribute.GetValueOrDefault()) : new EEpLicenseMode?();
      reader.Read();
    }

    public void WriteXml(XmlWriter writer)
    {
      writer.SetAttribute<int>("Platform", (int) this.Platform);
      if (!this.LicenseMode.HasValue)
        return;
      writer.SetAttribute<int>("Mode", (int) this.LicenseMode.Value);
    }
  }
}
