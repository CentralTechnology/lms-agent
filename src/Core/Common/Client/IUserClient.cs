namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Dependency;
    using Models;

    public interface IUserClient : ITransientDependency
    {
        Task Add(List<LicenseUser> users);
        Task Remove(List<LicenseUser> users);

        Task Update(List<LicenseUser> users);
    }

    public interface ISupportUploadClient : ITransientDependency
    {
        Task Add(SupportUpload upload);
        Task<SupportUpload> Get(int id);
        CallInStatus GetStatusByDeviceId(Guid deviceId);
        Task<int> GetUploadIdByDeviceId(Guid deviceId);

        Task Update(SupportUpload upload);
    }

    public interface IProfileClient : ITransientDependency
    {
        int GetAccountByDeviceId(Guid deviceId);
    }

    public interface IUserGroupClient : ITransientDependency
    {
        void Add(IList<LicenseUserGroup> groups);
        void Remove(IList<LicenseUserGroup> groups);

        void Update(IList<LicenseUserGroup> groups);
    }
}