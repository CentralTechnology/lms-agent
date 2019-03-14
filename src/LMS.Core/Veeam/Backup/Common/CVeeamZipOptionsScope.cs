using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [COptionsScope("VeeamZip")]
    public sealed class CVeeamZipOptionsScope
    {
        public CVeeamZipOptionsScope(IOptionsReader reader)
        {
            SOptionsLoader.LoadOptions<CVeeamZipOptionsScope>(this, reader);
        }

        [COptionsTimeSpan(1440, EOptionsTimeSpanFormat.Minutes, RelativeName = "RecheckCredentialsDelayMinutes")]
        public TimeSpan RecheckCredentialsDelay { get; set; }
    }
}
