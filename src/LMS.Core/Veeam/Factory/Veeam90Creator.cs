using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Factory
{
    class Veeam90Creator : VeeamCreator
    {
        public Veeam90Creator(Version applicationVersion) : base(applicationVersion)
        {
        }

        public override Portal.LicenseMonitoringSystem.Veeam.Entities.Veeam Create()
        {
            throw new NotImplementedException();
        }
    }
}
