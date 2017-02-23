namespace Core.Common
{
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;

    public static class PrincipleExtensions
    {
        public static string GetProperty(this Principal principal, string property)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry == null)
            {
                return string.Empty;
            }

            return directoryEntry.Properties.Contains(property) ? directoryEntry.Properties[property].Value.ToString() : string.Empty;
        }
    }
}