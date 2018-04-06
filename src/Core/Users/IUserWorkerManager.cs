namespace LMS.Users
{
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using global::Hangfire.Server;

    public interface IUserWorkerManager : IDomainService
    {
        Task ComputeGroupMembershipAsync(PerformContext performContext);
        Task ComputeGroups(PerformContext performContext);
        Task ComputeUsers(PerformContext performContext, int managedSupportId);
    }
}