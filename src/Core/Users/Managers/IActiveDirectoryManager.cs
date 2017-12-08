namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using Abp.Domain.Services;
    using Dto;
    using global::Hangfire.Server;

    public interface IActiveDirectoryManager : IDomainService
    {
        LicenseGroupDto GetGroup(PerformContext performContext, Guid groupId);
        LicenseGroupUsersDto GetGroupMembers(PerformContext performContext, Guid groupId);
        IEnumerable<LicenseGroupDto> GetGroups(PerformContext performContext);
        LicenseUserDto GetUser(PerformContext performContext, Guid userId);
        IEnumerable<LicenseUserDto> GetUsers(PerformContext performContext);
        bool IsOnDomain(PerformContext performContext);
        bool IsPrimaryDomainController(PerformContext performContext);
    }
}