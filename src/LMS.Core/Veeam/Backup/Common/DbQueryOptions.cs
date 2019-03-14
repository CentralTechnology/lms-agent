using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public sealed class DbQueryOptions
    {
        public bool IsRetryable { get; set; }

        public DbQueryOptions()
        {
            this.IsRetryable = true;
        }
    }
}
