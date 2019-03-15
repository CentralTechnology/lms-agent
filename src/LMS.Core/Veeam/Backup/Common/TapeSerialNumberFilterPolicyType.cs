using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public enum TapeSerialNumberFilterPolicyType
    {
        NoFilter,
        FilterInvalidSymbolsByScsi,
        FilterInvalidSymbolsByScsiWithSpaceTrimming,
        TrimSpaces,
    }
}
