namespace LMS.Users
{
    using Abp.Domain.Services;
    using Common.Interfaces;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IUserWorkerManager : IDomainService
    {
        void ProcessGroups(PerformContext performContext, ManagedSupport managedSupport);
        void ProcessUserGroups(PerformContext performContext);
        void ProcessUsers(PerformContext performContext, ManagedSupport managedSupport);
    }
}