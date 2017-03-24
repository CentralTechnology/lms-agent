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
        /// <param name="childProgressBar"></param>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups(ChildProgressBar childProgressBar);
    }
}