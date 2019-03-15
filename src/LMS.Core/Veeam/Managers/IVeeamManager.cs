namespace LMS.Core.Veeam.Managers
{
    using System;
    using Abp.Domain.Services;
    using global::Hangfire.Server;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public interface IVeeamManager: IDomainService
    {
        bool IsInstalled(PerformContext performContext);
        bool IsOnline();
        Version GetInstalledVeeamVersion();
    }
}
