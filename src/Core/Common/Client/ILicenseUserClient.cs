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
        Task Add(List<LicenseUser> users, ChildProgressBar childProgressBar);
        Task Remove(List<LicenseUser> users, ChildProgressBar childProgressBar);

        Task Update(List<LicenseUser> users, ChildProgressBar childProgressBar);

        Task<List<LicenseUser>> GetAll();
    }

    public interface ILicenseGroupClient : IDomainService
    {
        Task Add(List<LicenseGroup> groups, ChildProgressBar childProgressBar);
        Task Remove(List<LicenseGroup> groups, ChildProgressBar childProgressBar);

        Task Update(List<LicenseGroup> groups, ChildProgressBar childProgressBar);

        Task<List<LicenseGroup>> GetAll();
    }

    public interface ILicenseUserGroupClient : IDomainService
    {
        Task Add(List<LicenseUser> users, LicenseGroup @group, ChildProgressBar childProgressBar);
        Task Remove(List<LicenseUser> users, LicenseGroup @group, ChildProgressBar childProgressBar);
    }

    public interface ISupportUploadClient : IDomainService
    {
        Task<SupportUpload> Add(SupportUpload upload);
        Task<SupportUpload> Get(int id);
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