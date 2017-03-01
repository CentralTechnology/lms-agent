namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Models;
    using ShellProgressBar;

    public interface ILicenseUserClient : ITransientDependency
    {
        Task Add(List<LicenseUser> users, ChildProgressBar childProgressBar);
        Task Remove(List<LicenseUser> users, ChildProgressBar childProgressBar);

        Task Update(List<LicenseUser> users, ChildProgressBar childProgressBar);

        Task<List<LicenseUser>> GetAll();
    }

    public interface ILicenseGroupClient : ITransientDependency
    {
        Task Add(List<LicenseGroup> groups, ChildProgressBar childProgressBar);
        Task Remove(List<LicenseGroup> groups, ChildProgressBar childProgressBar);

        Task Update(List<LicenseGroup> groups, ChildProgressBar childProgressBar);

        Task<List<LicenseGroup>> GetAll();
    }

    public interface ILicenseUserGroupClient : ITransientDependency
    {
        Task Add(List<LicenseUser> users, LicenseGroup @group, ChildProgressBar childProgressBar);
        Task Remove(List<LicenseUser> users, LicenseGroup @group, ChildProgressBar childProgressBar);
    }

    public interface ISupportUploadClient : ITransientDependency
    {
        Task<SupportUpload> Add(SupportUpload upload);
        Task<SupportUpload> Get(int id);
        Task<CallInStatus> GetStatusByDeviceId(Guid deviceId);
        Task<int> GetUploadIdByDeviceId(Guid deviceId);

        Task<int> GetNewUploadId();

        Task Update(int id);

        Task<List<LicenseUser>> GetUsers(int uploadId);
    }

    public interface IProfileClient : ITransientDependency
    {
        Task<int> GetAccountByDeviceId(Guid deviceId);
    }


}