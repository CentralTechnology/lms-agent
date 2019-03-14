using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProductService : IProductProcess
    {
        int Order { get; }

        string ServiceName { get; }

        bool CanBeDisabled { get; }
    }
}
