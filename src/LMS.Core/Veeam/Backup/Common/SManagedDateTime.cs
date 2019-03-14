using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SManagedDateTime
    {
        public static DateTime Now
        {
            get
            {
                return DateTime.Now + SDebugTimeShiftHolder.GetShift();
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow + SDebugTimeShiftHolder.GetShift();
            }
        }

        public static DateTime Today
        {
            get
            {
                return (DateTime.Now + SDebugTimeShiftHolder.GetShift()).Date;
            }
        }
    }
}
