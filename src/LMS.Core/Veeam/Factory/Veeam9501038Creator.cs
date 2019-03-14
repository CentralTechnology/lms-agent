using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;

namespace LMS.Core.Veeam.Factory
{
    public class Veeam9501038Creator : VeeamCreator
    {
        public Veeam9501038Creator(Version applicationVersion) : base(applicationVersion)
        {
        }

        public override Result<Portal.LicenseMonitoringSystem.Veeam.Entities.Veeam> Create()
        {
            throw new NotImplementedException();
        }
    }
}
