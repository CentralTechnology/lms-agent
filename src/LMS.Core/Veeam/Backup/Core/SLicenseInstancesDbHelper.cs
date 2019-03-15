using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.DBManager;
using Serilog;

namespace LMS.Core.Veeam.Backup.Core
{
    public static class SLicenseInstancesDbHelper
    {
        //public static SqlLicensePlatformWeightsTableType CreateWeightsDbTable(
        //    IReadOnlyList<CLicensePlatformWeight> weights)
        //{
        //    SqlLicensePlatformWeightsTableType weightsTableType = new SqlLicensePlatformWeightsTableType();
        //    Log.Trace("License platform weights:");
        //    foreach (CLicensePlatformWeight weight in (IEnumerable<CLicensePlatformWeight>) weights)
        //    {
        //        weightsTableType.AddRow(weight.Platform.Platform, weight.Platform.LicenseMode, weight.Weight);
        //    }
        //    return weightsTableType;
        //}

        public static SqlLicensePlatformTableType CreatePlatformsDbTable(
            IReadOnlyList<CLicensePlatform> platforms)
        {
            SqlLicensePlatformTableType platformTableType = new SqlLicensePlatformTableType();
            Log.Debug("License platform weights:");
            foreach (CLicensePlatform platform in (IEnumerable<CLicensePlatform>) platforms)
            {
                platformTableType.AddRow(platform.Platform, platform.LicenseMode);
            }
            return platformTableType;
        }
    }
}
