using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Factory
{
    public class Veeam9542615Creator : VeeamCreator
    {
        public Veeam9542615Creator(Version applicationVersion) : base(applicationVersion)
        {
        }

        public override Portal.LicenseMonitoringSystem.Veeam.Entities.Veeam Create()
        {
            throw new NotImplementedException();
        }
    }
}
