namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;
    using global::Hangfire.Server;

    public interface IUserGroupManager : IDomainService
    {
        void AddUsersToGroup(PerformContext performContext, LicenseGroupUsersDto groupMembers);

        void DeleteUsersFromGroup(PerformContext performContext, LicenseGroupUsersDto groupMembers);
    }
}