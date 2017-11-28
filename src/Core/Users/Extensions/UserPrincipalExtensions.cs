using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Extensions
{
    using System.DirectoryServices.AccountManagement;

    public static class UserPrincipalExtensions
    {
        /// <summary>
        /// Get the name of the object as sometimes certian properties are not populated.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <returns></returns>
        public static string GetDisplayText(this UserPrincipal userPrincipal)
        {
            return userPrincipal.DisplayName ?? userPrincipal.Name ?? userPrincipal.SamAccountName;
        }
    }
}
