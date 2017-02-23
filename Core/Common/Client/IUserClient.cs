namespace LicenseMonitoringSystem.Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Portal.Common.Enums;
    using Portal.License.User;

    public interface IUserClient : ITransientDependency
    {
        void Add(IList<LicenseUser> users);
        void Remove(IList<LicenseUser> users);

        void Update(IList<LicenseUser> users);
    }

    public interface IUserUploadClient : ITransientDependency
    {
        void Add(LicenseUserUpload entity);
        LicenseUserUpload Get(int id);
        CallInStatus GetStatusByDeviceId(Guid deviceId);
        int GetUploadIdByDeviceId(Guid deviceId);

        void Update(int id, LicenseUserUpload userUpload);
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