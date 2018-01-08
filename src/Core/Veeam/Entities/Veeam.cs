using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.LicenseMonitoringSystem.Veeam.Entities
{
    public partial class Veeam
    {
        public override string ToString()
        {
            return $"Edition: {Edition}  License: {LicenseType}  Version: {ProgramVersion}  Hyper-V: {HyperV}  VMWare: {vSphere}";
        }
    }
}
