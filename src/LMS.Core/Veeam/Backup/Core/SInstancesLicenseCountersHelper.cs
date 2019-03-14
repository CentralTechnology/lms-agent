using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Common;

namespace LMS.Core.Veeam.Core
{
    public static class SInstancesLicenseCountersHelper
    {
        public static int GetProtectedVmsCount(CLicensePlatform platform)
        {
            using (SqlLicensePlatformTableType platformsDbTable = SLicenseInstancesDbHelper.CreatePlatformsDbTable((IReadOnlyList<CLicensePlatform>) new CLicensePlatform[1]
            {
                platform
            }))
                return CDBManager.Instance.Licensing.GetProtectedInstancesCounter(platformsDbTable);
        }
    }
}
