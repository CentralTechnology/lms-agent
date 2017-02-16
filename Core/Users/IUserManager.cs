using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Core.Users
{
    using Common.Portal.License;
    using Common.Portal.License.User;

    public interface IUserManager
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups();
    }
}
