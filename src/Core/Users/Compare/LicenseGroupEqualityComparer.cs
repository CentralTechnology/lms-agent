namespace LMS.Core.Users.Compare
{
    using System.Collections.Generic;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseGroupEqualityComparer : IEqualityComparer<LicenseGroup>
    {
        public bool Equals(LicenseGroup x, LicenseGroup y)
        {
            return y != null && x != null && x.Id == y.Id;
        }

        public int GetHashCode(LicenseGroup obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}