using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;

namespace LMS.Core.Veeam.Factory
{
    public abstract class VeeamCreator
    {
        protected readonly Version ApplicationVersion;

        protected VeeamCreator(Version applicationVersion)
        {
            ApplicationVersion = applicationVersion;
        }

        public abstract Result<Portal.LicenseMonitoringSystem.Veeam.Entities.Veeam> Create();
    }
}
