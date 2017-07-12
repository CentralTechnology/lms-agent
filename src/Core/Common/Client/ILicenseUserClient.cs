namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Models;

    public interface ILicenseUserClient : IDomainService
    {
        Task Add(List<LicenseUser> users);

        Task<List<LicenseUser>> GetAll();
        Task Remove(List<LicenseUser> users);

        Task Update(List<LicenseUser> users);
    }

    public interface ILicenseGroupClient : IDomainService
    {
        Task Add(List<LicenseGroup> groups);

        Task<List<LicenseGroup>> GetAll();
        Task Remove(List<LicenseGroup> groups);

        Task Update(List<LicenseGroup> groups);
    }

    public interface ILicenseUserGroupClient : IDomainService
    {
        Task Add(List<LicenseUser> users, LicenseGroup group);
        Task Remove(List<LicenseUser> users, LicenseGroup group);
    }

    public interface ISupportUploadClient : IDomainService
    {
        Task<ManagedSupport> Add(ManagedSupport upload);
        Task<ManagedSupport> Get(int id);

        Task<int> GetNewUploadId();
        Task<CallInStatus> GetStatusByDeviceId(Guid deviceId);
        Task<int> GetUploadIdByDeviceId(Guid deviceId);

        Task<List<LicenseUser>> GetUsers(int uploadId);

        Task Update(int id);
    }

    public interface IProfileClient : IDomainService
    {
        Task<int> GetAccountByDeviceId(Guid deviceId);
    }
}