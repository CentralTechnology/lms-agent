using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Compare
{
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
