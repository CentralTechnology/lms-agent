namespace LMS.Common.Extensions
{
    using System.DirectoryServices;
    using Users.Enums;

    public static class DirectoryEntryExtensions
    {
        public static bool IsAccountDisabled(this DirectoryEntry user)
        {
            const string uac = "userAccountControl";
            if (user.NativeGuid == null)
            {
                return false;
            }

            if (user.Properties[uac]?.Value != null)
            {
                var userFlags = (UserFlags) user.Properties[uac].Value;
                return userFlags.Contains(UserFlags.AccountDisabled);
            }

            return false;
        }
    }
}