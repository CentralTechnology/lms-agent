namespace Core.Users
{
    using System.Collections.Generic;

    public interface IUserManager
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<User> GetUsersAndGroups();
    }
}