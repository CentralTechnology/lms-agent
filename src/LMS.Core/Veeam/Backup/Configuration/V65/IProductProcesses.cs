using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProductProcesses : IEnumerable<IProductProcess>, IEnumerable
    {
        IProductProcess this[Guid id] { get; }

        IEnumerable<IProductProcessNode> Items { get; }
    }
}
