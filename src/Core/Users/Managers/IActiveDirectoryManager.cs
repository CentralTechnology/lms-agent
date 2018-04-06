namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using Abp.Domain.Services;
    using Dto;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IActiveDirectoryManager : IDomainService
    {
        LicenseGroup GetGroup(PerformContext performContext, Guid groupId);
        List<LicenseUserGroup> GetGroupMembers(PerformContext performContext, Guid groupId);
        IEnumerable<LicenseGroup> GetAllGroups(PerformContext performContext);
        List<LicenseGroup> GetAllGroupsList(PerformContext performContext);
        LicenseUser GetUserById(PerformContext performContext, Guid? userId);
        LicenseUser GetUserByPrincipalName(PerformContext performContext, string principalName);
        LicenseUser GetUser(PerformContext performContext, IdentityType type, string key);

        IEnumerable<LicenseUser> GetAllUsers(PerformContext performContext);
        List<LicenseUser> GetAllUsersList(PerformContext performContext);
        bool IsOnDomain(PerformContext performContext);
        bool IsPrimaryDomainController(PerformContext performContext);
    }
}