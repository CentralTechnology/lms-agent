using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [COptionsScope("Sobr")]
    public sealed class CSobrOptionsScope
    {
        [COptionsValue]
        public readonly bool IsFileCommanderCachingForced;
        [COptionsValue]
        public readonly bool IsFileCommanderCachingDisabled;

        public CSobrOptionsScope(IOptionsReader reader)
        {
            SOptionsLoader.LoadOptions<CSobrOptionsScope>(this, reader);
        }
    }
}
