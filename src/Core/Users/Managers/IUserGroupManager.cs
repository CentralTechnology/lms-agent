using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;

    public interface IUserGroupManager : IDomainService
    {
        void AddUsersToGroup(LicenseGroupUsersDto groupMembers);

        void DeleteUsersFromGroup(LicenseGroupUsersDto groupMembers);
    }
}
