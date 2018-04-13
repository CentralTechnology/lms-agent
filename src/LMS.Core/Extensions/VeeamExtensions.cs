namespace LMS.Core.Extensions
{
    using System;
    using Abp.UI;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public static class VeeamExtensions
    {
        public static void Validate(this Veeam veeam)
        {
            if (veeam.ExpirationDate == default(DateTime))
            {
                throw new UserFriendlyException($"Invalid Expiration Date: {veeam.ExpirationDate}");
            }

            if (veeam.Id == default(Guid))
            {
                throw new UserFriendlyException($"Invalid Id: {veeam.Id}");
            }

            if (veeam.TenantId == default(int))
            {
                throw new UserFriendlyException($"Invalid Account: {veeam.TenantId}");
            }
        }
    }
}