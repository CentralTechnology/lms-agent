using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProductProcess
    {
        Guid Id { get; }

        ProcessPlatform Platform { get; }

        ProcessType Type { get; }

        string Name { get; }

        string FileName { get; }

        ProductProcessOwner[] Owners { get; }

        string ProductCode { get; }

        string ComponentId { get; }

        string Description { get; }
    }
}
