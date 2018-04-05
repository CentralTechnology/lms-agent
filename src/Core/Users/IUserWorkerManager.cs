namespace LMS.Users
{
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IUserWorkerManager : IDomainService
    {
        Task ComputeGroupMembershipAsync(PerformContext performContext);
        Task ComputeGroups(PerformContext performContext);
        Task ComputeUsers(PerformContext performContext, int managedSupportId);
        //void ProcessGroups(PerformContext performContext, ManagedSupport managedSupport);
        //void ProcessUserGroups(PerformContext performContext);
        //void ProcessUsers(PerformContext performContext, ManagedSupport managedSupport);
    }
}