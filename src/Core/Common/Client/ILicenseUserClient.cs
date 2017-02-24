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

    public interface ISupportUploadClient : ITransientDependency
    {
        Task Add(SupportUpload upload);
        Task<SupportUpload> Get(int id);
        Task<CallInStatus> GetStatusByDeviceId(Guid deviceId);
        Task<int> GetUploadIdByDeviceId(Guid deviceId);

        Task Update(SupportUpload upload);

        Task<List<LicenseUser>> GetUsers(int uploadId);
    }

    public interface IProfileClient : ITransientDependency
    {
        Task<int> GetAccountByDeviceId(Guid deviceId);
    }


}