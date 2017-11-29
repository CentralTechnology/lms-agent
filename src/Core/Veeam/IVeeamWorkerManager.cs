namespace LMS.Veeam
{
    using Abp.Domain.Services;

    public interface IVeeamWorkerManager : IDomainService
    {
        void Start();
    }
}