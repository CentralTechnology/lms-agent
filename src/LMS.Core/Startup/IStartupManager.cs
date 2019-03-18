namespace LMS.Core.StartUp
{
    using Abp.Domain.Services;
    using global::Hangfire.Server;

    public interface IStartupManager : IDomainService
    {
        bool Init();
        bool ShouldMonitorUsers();
        bool MonitorVeeam();
    }
}