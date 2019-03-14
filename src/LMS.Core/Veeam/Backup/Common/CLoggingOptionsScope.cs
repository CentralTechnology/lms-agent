using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [COptionsScope("Logging")]
    public sealed class CLoggingOptionsScope
    {
        [COptionsValue(RelativeName = "DatabaseLoggingLevel")]
        private int DatabaseLoggingLevel_Internal = 1;

        public CLoggingOptionsScope(IOptionsReader reader)
        {
            SOptionsLoader.LoadOptions<CLoggingOptionsScope>(this, reader);
        }

        public EDatabaseLoggingLevel DatabaseLoggingLevel
        {
            get
            {
                return (EDatabaseLoggingLevel) this.DatabaseLoggingLevel_Internal;
            }
        }
    }
}
