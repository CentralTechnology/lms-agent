﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Compare
{
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseGroupComparer : IEqualityComparer<LicenseGroup>
    {
        /// <inheritdoc />
        public bool Equals(LicenseGroup x, LicenseGroup y)
        {
            return x.Id == y.Id;
        }

        /// <inheritdoc />
        public int GetHashCode(LicenseGroup obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
