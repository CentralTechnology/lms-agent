namespace LMS.Core.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using Abp.Domain.Services;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IActiveDirectoryManager : IDomainService
    {
        IEnumerable<LicenseGroup> GetAllGroups();
        List<LicenseGroup> GetAllGroupsList();

        IEnumerable<LicenseUser> GetAllUsers();
        List<LicenseUser> GetAllUsersList();
        LicenseGroup GetGroup(Guid groupId);
        List<LicenseUserGroup> GetGroupMembers(Guid groupId);
        LicenseUser GetUser(IdentityType type, string key);
        LicenseUser GetUserById(Guid userId);
        LicenseUser GetUserByPrincipalName(string principalName);
        bool IsOnDomain(PerformContext performContext);
        bool IsPrimaryDomainController(PerformContext performContext);
    }
}