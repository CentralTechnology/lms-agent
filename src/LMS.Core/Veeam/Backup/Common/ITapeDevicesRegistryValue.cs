using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public interface ITapeDevicesRegistryValue
    {
        bool IsManualMatchSetByUser { get; }

        bool ContainsMatching(string tapeServerName, string changerSysName);

        IEnumerable<CTapeDriveRegValue> GetDrives(
            string tapeServerName,
            string changerName);
    }
}
