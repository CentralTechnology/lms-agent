namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using Abp.Domain.Services;
    using Dto;
    using global::Hangfire.Server;

    public interface IActiveDirectoryManager : IDomainService
    {
        LicenseGroupDto GetGroup(PerformContext performContext, Guid groupId);
        LicenseGroupUsersDto GetGroupMembers(PerformContext performContext, Guid groupId);
        IEnumerable<LicenseGroupDto> GetGroups(PerformContext performContext);
        LicenseUserDto GetUserById(PerformContext performContext, Guid? userId);
        LicenseUserDto GetUserByPrincipalName(PerformContext performContext, string principalName);
        LicenseUserDto ValidateUser(PerformContext performContext, UserPrincipal princiapl);
        LicenseUserDto GetUser(PerformContext performContext, IdentityType type, string key);

        IEnumerable<LicenseUserDto> GetUsers(PerformContext performContext);
        bool IsOnDomain(PerformContext performContext);
        bool IsPrimaryDomainController(PerformContext performContext);
    }
}