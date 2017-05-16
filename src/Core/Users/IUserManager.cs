namespace Core.Users
{
    using System.Collections.Generic;
    using Abp.Dependency;
    using Abp.Domain.Services;
    using Models;
    using ShellProgressBar;

    public interface IUserManager : IDomainService
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups();
    }
}