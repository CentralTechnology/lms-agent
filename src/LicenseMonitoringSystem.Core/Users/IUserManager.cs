namespace Core.Users
{
    using System.Collections.Generic;
    using Abp.Dependency;
    using Models;
    using ShellProgressBar;

    public interface IUserManager : ITransientDependency
    {
        /// <summary>
        /// </summary>
        /// <param name="childProgressBar"></param>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups(ChildProgressBar childProgressBar);
    }
}