using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [COptionsScope("CloudConnectBin")]
    public sealed class CCloudBinOptionsScope
    {
        [COptionsValue]
        public readonly string RepositoryFolderName = "_RecycleBin";
        [COptionsValue]
        public readonly int GfsNotificationSeverity = -1;
        [COptionsValue]
        public readonly bool IsRetentionDisabled;
        [COptionsValue]
        public readonly bool IsEvacuationDisabled;

        public CCloudBinOptionsScope(IOptionsReader reader)
        {
            SOptionsLoader.LoadOptions<CCloudBinOptionsScope>(this, reader);
        }

        [COptionsTimeSpan(20, EOptionsTimeSpanFormat.Minutes, RelativeName = "RetainRescanIntervalMinutes")]
        public TimeSpan RetainRescanInterval { get; set; }

        [COptionsTimeSpan(60, EOptionsTimeSpanFormat.Minutes, RelativeName = "RetainHeartbeatMinutes")]
        public TimeSpan RetainHeartbeatInterval { get; set; }
    }
}
