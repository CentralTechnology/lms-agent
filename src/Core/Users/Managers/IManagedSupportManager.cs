namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IManagedSupportManager : IDomainService
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        ManagedSupport Add();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        ManagedSupport Get();

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        void Update(ManagedSupport input);
    }
}