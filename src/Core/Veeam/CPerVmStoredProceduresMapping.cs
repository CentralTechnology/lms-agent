using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam
{
    internal class CPerVmStoredProceduresMapping : IPerVmStoredProceduresMapping
    {
        public string GetProtectedVms => "[dbo].[GetProtectedVms]";

        public string GetVmsNumbers => "[dbo].[GetVmsNumbers]";

        public string CanProcessVm => "[dbo].[CanProcessVm]";

        public string GetPerVmRestorePointsData => "[dbo].[GetPerVmRestorePointsData]";
    }
}
