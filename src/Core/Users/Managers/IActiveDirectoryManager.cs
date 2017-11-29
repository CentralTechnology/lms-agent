using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;

    public interface IActiveDirectoryManager : IDomainService
    {
        IEnumerable<LicenseUserDto> GetUsers();
        LicenseUserDto GetUser(Guid userId);
        IEnumerable<LicenseGroupDto> GetGroups();
        LicenseGroupDto GetGroup(Guid groupId);        
        LicenseGroupUsersDto GetGroupMembers(Guid groupId);
        bool IsOnDomain();
        bool IsPrimaryDomainController();
    }
}
