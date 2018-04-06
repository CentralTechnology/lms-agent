namespace LMS.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Microsoft.OData.Client;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public interface IPortalService : IDomainService
    {
        Task AddGroupAsync(LicenseGroup group);
        Task AddManagedServerAsync(ManagedSupport managedSupport);
        Task AddUserAsync(LicenseUser user);
        Task AddUserGroupAsync(LicenseUserGroup userGroup);
        Task DeleteGroupAsync(LicenseGroup group);
        Task DeleteUserAsync(LicenseUser user);
        Task DeleteUserGroupAsync(LicenseUserGroup userGroup);
        Task<List<LicenseGroup>> GetAllGroupsAsync();
        List<LicenseUserGroup> GetAllGroupUsers(Guid @group);
        Task<List<LicenseUser>> GetAllUsersAsync();
        ManagedSupport GetManagedServer();
        DataServiceCollection<Veeam> GetVeeamServer();
        DataServiceCollection<LicenseUser> GetUserById(Guid userId);
        Task UpdateGroupAsync(LicenseGroup group);
        Task UpdateManagedServerAsync(ManagedSupport update);
        Task UpdateUserAsync(LicenseUser user);
        Task UpdateVeeamServerAsync(Veeam update);
    }
}