namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using Abp.Domain.Services;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IActiveDirectoryManager : IDomainService
    {
        IEnumerable<LicenseGroup> GetAllGroups(PerformContext performContext);
        List<LicenseGroup> GetAllGroupsList(PerformContext performContext);

        IEnumerable<LicenseUser> GetAllUsers(PerformContext performContext);
        List<LicenseUser> GetAllUsersList(PerformContext performContext);
        LicenseGroup GetGroup(PerformContext performContext, Guid groupId);
        List<LicenseUserGroup> GetGroupMembers(PerformContext performContext, Guid groupId);
        LicenseUser GetUser(PerformContext performContext, IdentityType type, string key);
        LicenseUser GetUserById(PerformContext performContext, Guid? userId);
        LicenseUser GetUserByPrincipalName(PerformContext performContext, string principalName);
        bool IsOnDomain(PerformContext performContext);
        bool IsPrimaryDomainController(PerformContext performContext);
    }
}