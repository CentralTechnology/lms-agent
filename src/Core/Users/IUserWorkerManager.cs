namespace LMS.Users
{
    using Abp.Domain.Services;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IUserWorkerManager : IDomainService
    {
        void ProcessGroups(ManagedSupport managedSupport);
        void ProcessUserGroups();
        void ProcessUsers(ManagedSupport managedSupport);
        void Start();
    }
}