using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IDatabaseVersion
    {
        DatabaseVersionType[] Types { get; }

        int this[DatabaseVersionType type] { get; }
    }
}
