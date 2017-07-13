namespace Core.Users
{
    using System.Collections.Generic;
    using Abp.Domain.Services;
    using Models;

    public interface IUserManager : IDomainService
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<LicenseUser> GetUsersAndGroups();
    }
}