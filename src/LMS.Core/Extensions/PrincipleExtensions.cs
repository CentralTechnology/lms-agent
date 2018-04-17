namespace LMS.Core.Extensions
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using Abp.UI;

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

        public static UserPrincipal Validate(this Principal principal)
        {
            if (!principal.Guid.HasValue)
            {
                throw new UserFriendlyException($"Cannot process {principal.Name} because the Id doesn't contain a value. Please check this manually in Active Directory.");
            }

            bool validId = Guid.TryParse(principal.Guid.ToString(), out Guid principalId);
            if (!validId)
            {
                throw new UserFriendlyException($"Cannot process {principal.Name} because the Id is not valid. Please check this manually in Active Directory.");
            }

            if (principal is UserPrincipal user)
            {
                return user;
            }

            throw new UserFriendlyException($"User is not of type UserPrincipal.");
        }
    }
}