using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Common.Helpers
{
    using Abp;
    using Extensions;
    using Microsoft.Win32;

    public static class CommonHelpers
    {
        private static readonly RegistryKey[] UninstallKeys = {
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
        };

        /// <summary>
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static bool IsApplictionInstalled(string pName)
        {
            foreach (var key in UninstallKeys)
            {
                if (key != null)
                {
                    var data = key.GetSubKeyValue(key.GetSubKeyNames(), new NameValue("DisplayName", pName));
                    if (data.exist)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
