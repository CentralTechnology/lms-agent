using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    [Flags]
    public enum RegistryKeyState
    {
        Dirty = 1,
        SystemKey = 2,
        WriteAccess = 4,
        PerfData = 8,
    }
}
