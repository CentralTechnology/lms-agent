namespace LMS.Core.Users.Compare
{
    using System.Collections.Generic;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseUserGroupEqualityComparer : IEqualityComparer<LicenseUserGroup>
    {
        public bool Equals(LicenseUserGroup x, LicenseUserGroup y)
        {
            return y != null && x != null && x.UserId == y.UserId && x.GroupId == y.GroupId;
        }

        public int GetHashCode(LicenseUserGroup obj)
        {
            return new {obj.UserId, obj.GroupId}.GetHashCode();
        }
    }
}