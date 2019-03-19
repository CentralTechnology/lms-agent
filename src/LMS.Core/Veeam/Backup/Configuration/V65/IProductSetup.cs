using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProductSetup
    {
        Version Version { get; }

        ProcessPlatform Platform { get; }

        string FileName { get; }

        Guid PackageCode { get; }

        Guid ProductCode { get; }

        Guid UpgradeCode { get; }

        bool IsCurrent { get; }
    }
}
