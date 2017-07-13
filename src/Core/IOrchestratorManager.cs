namespace Core
{
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Common.Enum;

    public interface IOrchestratorManager : IDomainService
    {
        /// <summary>
        /// </summary>
        /// <param name="monitor"></param>
        void Run(Monitor monitor);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        Task UserMonitor();
    }
}