using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Extensions
{
    using System.DirectoryServices.AccountManagement;

    public static class GroupPrincipalExtensions
    {
        /// <summary>
        /// Get the name of the object as sometimes certian properties are not populated.
        /// </summary>
        /// <param name="groupPrincipal"></param>
        /// <returns></returns>
        public static string GetDisplayText(this GroupPrincipal groupPrincipal)
        {
            return groupPrincipal.DisplayName ?? groupPrincipal.SamAccountName;
        }
    }
}
