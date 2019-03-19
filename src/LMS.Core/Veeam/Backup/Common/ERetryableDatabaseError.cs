using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public enum ERetryableDatabaseError
    {
        SqlTimeout = -2,
        SqlConnectionBroken = -1,
        SqlOutOfMemory = 701, // 0x000002BD
        SqlOutOfLocks = 1204, // 0x000004B4
        SqlDeadlock = 1205, // 0x000004B5
        SqlLockRequestTimeout = 1222, // 0x000004C6
        SqlTimeoutWaitingForMemoryResource = 8645, // 0x000021C5
        SqlLowMemoryCondition = 8651, // 0x000021CB
        ThrownFromStoredProcedure = 50000, // 0x0000C350
    }
}
