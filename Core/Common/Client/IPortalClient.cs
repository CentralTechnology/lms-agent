namespace LicenseMonitoringSystem.Core.Common.Client
{
    using System;
    using Abp.Dependency;
    using Portal.Common.Enums;
    using Portal.License.User;

    public interface IPortalClient : ISingletonDependency
    {
        LicenseUserUpload Get(int id);

        int? GetAccountId(Guid deviceId);

        int GetId(Guid deviceId);

        CallInStatus GetStatus(Guid deviceId);

        int GetUploadId(DateTime? date);

        void Post(LicenseUserUpload entity);

        void Put(int id, LicenseUserUpload entity);
    }
}