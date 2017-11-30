namespace LMS.Common.Extensions
{
    using System;
    using Abp;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public static class VeeamExtensions
    {
        public static void Validate(this Veeam veeam)
        {
            if (veeam.ExpirationDate == default(DateTime))
            {
                throw new AbpException($"Invalid Expiration Date: {veeam.ExpirationDate}");
            }

            if (veeam.Id == default(Guid))
            {
                throw new AbpException($"Invalid Id: {veeam.Id}");
            }

            if (veeam.TenantId == default(int))
            {
                throw new AbpException($"Invalid Account: {veeam.TenantId}");
            }
        }
    }
}