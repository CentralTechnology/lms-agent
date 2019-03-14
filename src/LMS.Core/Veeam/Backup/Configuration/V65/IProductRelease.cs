using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProductRelease
    {
        string Name { get; }

        Version Version { get; }

        int UpdateNumber { get; }

        bool IsSupported { get; }

        bool IsCurrent { get; }

        int this[DatabaseVersionType type] { get; }
    }
}
