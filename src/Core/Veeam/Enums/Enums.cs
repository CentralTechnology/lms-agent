using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam.Enums
{
    public enum ELicenseGeneration
    {
        Old,
        V9,
    }

    public enum EVmLicensingStatus
    {
        NotRegistered,
        Expired,
        Managed
    }
}
