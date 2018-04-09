namespace LMS.Core.Managers
{
    using Abp.Domain.Services;
    using SharpRaven;

    public class LMSManagerBase : DomainService
    {
        public LMSManagerBase()
        {
            RavenClient = Sentry.RavenClient.Instance;
        }

        public RavenClient RavenClient { get; set; }
    }
}