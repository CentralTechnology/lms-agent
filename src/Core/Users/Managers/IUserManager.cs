using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IUserManager : IDomainService
    {
        IEnumerable<LicenseUser> AllUsers();
    }
}
