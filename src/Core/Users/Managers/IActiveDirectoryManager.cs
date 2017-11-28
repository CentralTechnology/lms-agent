using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IActiveDirectoryManager : IDomainService
    {
        IEnumerable<LicenseUser> AllUsers();

        IEnumerable<LicenseUserDto> GetUsers();

        IEnumerable<LicenseGroupDto> GetGroups();
    }
}
