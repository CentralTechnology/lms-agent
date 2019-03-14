using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Factory
{
    public abstract class VeeamCreator
    {
        protected readonly Version ApplicationVersion;

        protected VeeamCreator(Version applicationVersion)
        {
            ApplicationVersion = applicationVersion;
        }

        public abstract Portal.LicenseMonitoringSystem.Veeam.Entities.Veeam Create();
    }
}
