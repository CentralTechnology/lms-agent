using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    
    public class ProductProcessOwner
    {
        public Guid Id { get; private set; }

        public ProcessPlatform Platform { get; private set; }

        public ProductProcessOwner(Guid id, ProcessPlatform platform)
        {
            this.Id = id;
            this.Platform = platform;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", (object) this.Id.ToString("B").ToUpper(), (object) this.Platform);
        }
    }
}
