namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Abp.Domain.Services;
    using Models;
    using ShellProgressBar;

    public interface ILicenseUserClient : IDomainService
    {
        Task Add(List<LicenseUser> users);
        Task Remove(List<LicenseUser> users);

        Task Update(List<LicenseUser> users);

        Task<List<LicenseUser>> GetAll();
    }

    public interface ILicenseGroupClient : IDomainService
    {
        Task Add(List<LicenseGroup> groups);
        Task Remove(List<LicenseGroup> groups);

        Task Update(List<LicenseGroup> groups);

        Task<List<LicenseGroup>> GetAll();
    }

    public interface ILicenseUserGroupClient : IDomainService
    {
        Task Add(List<LicenseUser> users, LicenseGroup @group);
        Task Remove(List<LicenseUser> users, LicenseGroup @group);
    }

    public interface ISupportUploadClient : IDomainService
    {
        Task<ManagedSupport> Add(ManagedSupport upload);
        Task<ManagedSupport> Get(int id);
        Task<CallInStatus> GetStatusByDeviceId(Guid deviceId);
        Task<int> GetUploadIdByDeviceId(Guid deviceId);

        Task<int> GetNewUploadId();

        Task Update(int id);

        Task<List<LicenseUser>> GetUsers(int uploadId);
    }

    public interface IProfileClient : IDomainService
    {
        Task<int> GetAccountByDeviceId(Guid deviceId);
    }


}