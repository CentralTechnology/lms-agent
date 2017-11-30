namespace LMS.Users.Managers
{
    using System;
    using Abp.Domain.Services;
    using Dto;

    public interface IGroupManager : IDomainService
    {
        void Add(LicenseGroupDto input, int tenantId);
        void Delete(Guid id);
        void Update(LicenseGroupDto input);
    }
}