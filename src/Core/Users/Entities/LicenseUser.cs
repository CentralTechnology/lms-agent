namespace Portal.LicenseMonitoringSystem.Users.Entities
{
    using System;

    public partial class LicenseUser
    {
        public static LicenseUser Create(LicenseUser user, int managedSupportId, long tenantId)
        {
            return new LicenseUser
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Enabled = user.Enabled,
                FirstName = user.FirstName,
                Id = user.Id,
                IsDeleted = false,
                LastLoginDate = user.LastLoginDate,
                ManagedSupportId = managedSupportId,
                SamAccountName = user.SamAccountName,
                Surname = user.Surname,
                TenantId = Convert.ToInt32(tenantId)
            };
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(Surname))
            {
                return $"{FirstName} {Surname}";
            }

            if (!string.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }

            if (!string.IsNullOrEmpty(SamAccountName))
            {
                return SamAccountName;
            }

            return Id.ToString();
        }

        public void UpdateValues(LicenseUser user)
        {
            DisplayName = user.DisplayName;
            Email = user.Email;
            Enabled = user.Enabled;
            FirstName = user.FirstName;
            IsDeleted = false; // cannot be deleted if its in ad!
            LastLoginDate = user.LastLoginDate;
            SamAccountName = user.SamAccountName;
            Surname = user.Surname;
        }
    }
}