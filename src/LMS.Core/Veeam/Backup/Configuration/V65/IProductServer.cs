using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProductServer
    {
        ServerFamily Family { get; }

        int ServicePack { get; }

        Version Version { get; }

        int CompatibilityLevel { get; }

        string ToString(string format);
    }
}
