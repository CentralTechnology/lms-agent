namespace LMS.Users.Compare
{
    using System.Collections.Generic;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseUserComparer : IEqualityComparer<LicenseUser>
    {
        /// <inheritdoc />
        public bool Equals(LicenseUser x, LicenseUser y)
        {
            return x.Id == y.Id;
        }

        /// <inheritdoc />
        public int GetHashCode(LicenseUser obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}