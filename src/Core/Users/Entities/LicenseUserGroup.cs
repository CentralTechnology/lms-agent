using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.LicenseMonitoringSystem.Users.Entities
{
    public partial class LicenseUserGroup
    {
        public static LicenseUserGroup Create(Guid groupId, Guid userId)
        {
            return new LicenseUserGroup
            {
                GroupId = groupId,
                UserId = userId
            };
        }
    }
}
