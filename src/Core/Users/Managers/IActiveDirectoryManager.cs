namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using Abp.Domain.Services;
    using Dto;

    public interface IActiveDirectoryManager : IDomainService
    {
        LicenseGroupDto GetGroup(Guid groupId);
        LicenseGroupUsersDto GetGroupMembers(Guid groupId);
        IEnumerable<LicenseGroupDto> GetGroups();
        LicenseUserDto GetUser(Guid userId);
        IEnumerable<LicenseUserDto> GetUsers();
        bool IsOnDomain();
        bool IsPrimaryDomainController();
    }
}