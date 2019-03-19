using System.Collections.Generic;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.DBManager;
using LMS.Core.Veeam.DBManager;

namespace LMS.Core.Veeam.Backup.Core
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
