using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam
{
    using System.Diagnostics.CodeAnalysis;

    public class Veeam
    {
        public DateTime ExpirationDate { get; set; }
        public string ExternalIpAddress { get; set; }
        public string Hostname { get; set; }
        public int HyperV { get; set; }
        public LicenseTypeEx LicenseType { get; set; }
        public EPlatformFlags Platform { get; set; }
        public string ProgramVersion { get; set; }
        public string SupportId { get; set; }
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
}
