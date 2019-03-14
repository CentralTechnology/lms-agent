using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Common;

namespace LMS.Core.Veeam.Backup.Model
{
    public interface ILicensedInstance
    {
        Guid InstanceId { get; }

        CLicensePlatform Platform { get; }

        DateTime LastProcessingTime { get; }

        string Uuid { get; }

        string DnsName { get; }

        string ToLog();
    }
}
