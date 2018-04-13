namespace LMS.Core.Users.Compare
{
    using System.Collections.Generic;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseUserEqualityComparer : IEqualityComparer<LicenseUser>
    {
        public bool Equals(LicenseUser x, LicenseUser y)
        {
            return y != null && (x != null && x.Id == y.Id);
        }

        public int GetHashCode(LicenseUser obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}