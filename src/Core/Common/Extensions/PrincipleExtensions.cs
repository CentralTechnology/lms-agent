namespace LMS.Common.Extensions
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using Abp.Logging;
    using Core.Common.Extensions;
    using global::Hangfire.Server;

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

        public static UserPrincipal Validate(this Principal principal, PerformContext performContext)
        {
            if (!principal.Guid.HasValue)
            {
                LogHelper.Logger.Debug(performContext, $"Cannot process {principal.Name} because the Id doesn't contain a value. Please check this manually in Active Directory.");
                return null;
            }

            bool validId = Guid.TryParse(principal.Guid.ToString(), out Guid principalId);
            if (!validId)
            {
                LogHelper.Logger.Debug(performContext, $"Cannot process {principal.Name} because the Id is not valid. Please check this manually in Active Directory.");
                return null;
            }

            if (principal is UserPrincipal user)
            {
                return user;
            }

            return null;
        }
    }
}