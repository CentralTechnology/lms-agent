using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam
{
    interface IPerVmStoredProceduresMapping
    {
        string GetProtectedVms { get; }

        string GetVmsNumbers { get; }

        string CanProcessVm { get; }

        string GetPerVmRestorePointsData { get; }
    }
}
