namespace LMS.Startup
{
    using Abp.Domain.Services;

    public interface IStartupManager : IDomainService
    {
        bool Init();
        bool MonitorUsers();
        bool MonitorVeeam();
        bool ValidateCredentials();
    }
}