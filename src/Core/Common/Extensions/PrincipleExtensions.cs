namespace LMS.Common.Extensions
{
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;

    public static class PrincipleExtensions
    {
        public static string GetProperty(this Principal principal, string property)
        {
            if (!(principal.GetUnderlyingObject() is DirectoryEntry directoryEntry))
            {
                return string.Empty;
            }

            return directoryEntry.Properties.Contains(property) ? directoryEntry.Properties[property].Value.ToString() : string.Empty;
        }
    }
}