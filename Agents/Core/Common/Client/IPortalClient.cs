namespace LicenseMonitoringSystem.Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using Abp.Dependency;
    using Abp.WebApi.Client;
    using Portal.Common.Enums;
    using Portal.License;
    using Portal.License.User;

    public interface IPortalClient : ISingletonDependency
    {
        LicenseUserUpload Get(int id);

        int GetId(Guid deviceId);

        int GetUploadId(DateTime? date);

        void Post(LicenseUserUpload entity);

        void Put(int id, LicenseUserUpload entity);

        CallInStatus GetStatus(Guid deviceId);

        int? GetAccountId(Guid deviceId);
    }
}