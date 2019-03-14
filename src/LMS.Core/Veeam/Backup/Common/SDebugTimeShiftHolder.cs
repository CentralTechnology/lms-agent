using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SDebugTimeShiftHolder
    {
        private static readonly object _lock = new object();
        private static TimeSpan _shift;
        private static volatile bool _isSet;
        private static volatile bool _isLiveUpdate;

        public static TimeSpan GetShift()
        {
            if (!SDebugTimeShiftHolder._isSet)
                SDebugTimeShiftHolder._isLiveUpdate = SDebugTimeShiftHolder.GetIsLiveUpdate();
            if (SDebugTimeShiftHolder._isSet && !SDebugTimeShiftHolder._isLiveUpdate)
                return SDebugTimeShiftHolder._shift;
            lock (SDebugTimeShiftHolder._lock)
            {
                if (SDebugTimeShiftHolder._isSet && !SDebugTimeShiftHolder._isLiveUpdate)
                    return SDebugTimeShiftHolder._shift;
                SDebugTimeShiftHolder._shift = SDebugTimeShiftHolder.PrivateGetShift();
                SDebugTimeShiftHolder._isSet = true;
                return SDebugTimeShiftHolder._shift;
            }
        }

        private static bool GetIsLiveUpdate()
        {
            try
            {
                using (RegistryKey subKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).CreateSubKey("SOFTWARE\\Veeam\\Veeam Backup and Replication", RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (subKey == null)
                        return false;
                    return subKey.GetValue("DebugShiftTimeInMinutes") != null;
                }
            }
            catch
            {
                return false;
            }
        }

        [DebuggerStepThrough]
        private static TimeSpan PrivateGetShift()
        {
            try
            {
                using (RegistryKey subKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).CreateSubKey("SOFTWARE\\Veeam\\Veeam Backup and Replication", RegistryKeyPermissionCheck.ReadSubTree))
                    return subKey == null ? TimeSpan.Zero : TimeSpan.FromMinutes((double) Convert.ToInt32(subKey.GetValue("DebugShiftTimeInMinutes", (object) 0)));
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }
    }
}
