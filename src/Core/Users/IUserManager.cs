namespace Core.Users
{
    using System.Collections.Generic;
    using Models;

    public interface IUserManager
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups();
    }
}