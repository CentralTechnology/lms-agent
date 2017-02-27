namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Models;

    public interface ILicenseUserClient : ITransientDependency
    {
        Task Add(List<LicenseUser> users);
        Task Remove(List<LicenseUser> users);

        Task Update(List<LicenseUser> users);
    }

    public interface ILicenseGroupClient : ITransientDependency
    {
        Task Add(List<LicenseGroup> groups);
        Task Remove(List<LicenseGroup> groups);

        Task Update(List<LicenseGroup> groups);

        Task<List<LicenseGroup>> GetAll();
    }

    public interface ISupportUploadClient : ITransientDependency
    {
        Task<SupportUpload> Add(SupportUpload upload);
        Task<SupportUpload> Get(int id);
        Task<CallInStatus> GetStatusByDeviceId(Guid deviceId);
        Task<int> GetUploadIdByDeviceId(Guid deviceId);

        Task Update(int uploadId);

        Task<List<LicenseUser>> GetUsers(int uploadId);
    }

    public interface IProfileClient : ITransientDependency
    {
        Task<int> GetAccountByDeviceId(Guid deviceId);
    }


}