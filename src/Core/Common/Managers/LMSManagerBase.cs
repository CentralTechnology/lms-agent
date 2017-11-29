using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Common.Managers
{
    using Abp.Domain.Services;
    using SharpRaven;

    public class LMSManagerBase : DomainService
    {
        public LMSManagerBase()
        {
            RavenClient = Core.Sentry.RavenClient.Instance;
        }

        public RavenClient RavenClient { get; set; }
    }
}
