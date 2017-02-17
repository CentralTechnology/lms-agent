namespace LicenseMonitoringSystem.Core.Users
{
    using System.Collections.Generic;
    using Common.Portal.License.User;

    public interface IUserManager
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups();
    }
}