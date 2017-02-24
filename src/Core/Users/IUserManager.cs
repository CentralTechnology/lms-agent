namespace Core.Users
{
    using System.Collections.Generic;
    using Abp.Dependency;
    using Models;

    public interface IUserManager : ITransientDependency
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups();
    }
}