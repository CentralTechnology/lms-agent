using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductReleaseAttributes
    {
        public Version Upgdarable { get; private set; }

        public bool Preview { get; private set; }

        public ProductReleaseAttributes(Version upgradable, bool preview)
        {
            this.Upgdarable = upgradable;
            this.Preview = preview;
        }
    }
}
