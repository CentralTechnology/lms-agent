using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    using System.DirectoryServices;
    using Models;

    public static class DirectoryEntryExtensions
    {
        public static bool IsAccountDisabled(this DirectoryEntry user)
        {
            const string uac = "userAccountControl";
            if (user.NativeGuid == null) return false;

            if (user.Properties[uac]?.Value != null)
            {
                var userFlags = (UserFlags)user.Properties[uac].Value;
                return userFlags.Contains(UserFlags.AccountDisabled);
            }

            return false;
        }
    }
}
