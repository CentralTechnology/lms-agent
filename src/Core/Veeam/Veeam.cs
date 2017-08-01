namespace Core.Veeam
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Veeam
    {
        public LicenseEditions Edition { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ExternalIpAddress { get; set; }
        public string Hostname { get; set; }
        public int HyperV { get; set; }
        public Guid Id { get; set; }
        public LicenseTypeEx LicenseType { get; set; }
        public EPlatformFlags Platform { get; set; }
        public string ProgramVersion { get; set; }
        public string SupportId { get; set; }
        public int TenantId { get; set; }
        public int vSphere { get; set; }
    }

    [Flags]
    public enum EPlatformFlags
    {
        None = 0,
        Vmware = 1,
        HyperV = 2,
        Vcd = 16,
        Endpoint = 64,
        Any = Vcd | HyperV | Vmware
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LicenseTypeEx
    {
        Empty,
        Evalution,
        Full,
        NFR,
        Perpetual,
        Rental,
        Subscription
    }

    public enum LicenseEditions
    {
        Standard,
        Enterprise,
        EnterprisePlus
    }
}