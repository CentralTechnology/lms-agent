namespace Portal.LicenseMonitoringSystem.Users.Entities
{
    using System;
    using LMS.Users.Dto;

    public partial class LicenseGroup
    {
        public static LicenseGroup Create(LicenseGroup group, long tenantId)
        {
            return new LicenseGroup
            {
                Id = group.Id,
                Name = group.Name,
                TenantId = Convert.ToInt32(tenantId),
                WhenCreated = group.WhenCreated
            };
        }

        public void UpdateValues(LicenseGroup group)
        {
            Name = group.Name;
            WhenCreated = group.WhenCreated;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            return Id.ToString();
        }
    }
}