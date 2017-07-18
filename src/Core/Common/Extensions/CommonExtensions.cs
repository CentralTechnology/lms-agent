using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    using Abp.Extensions;
    using Microsoft.Win32;

    public static class CommonExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static bool IsApplictionInstalled(this string pName)
        {
            string displayName;

            // search in: CurrentUser
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                displayName = key.GetDisplayName();
                if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                displayName = key.GetDisplayName();
                if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // search in: LocalMachine_64
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                displayName = key.GetDisplayName();
                if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // NOT FOUND
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetDisplayName(this RegistryKey key)
        {
            foreach (string keyName in key.GetSubKeyNames())
            {
                RegistryKey subKey = key.OpenSubKey(keyName);
                if (subKey != null)
                {
                    return subKey.GetValue("DisplayName") as string;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static Version GetApplicationVersion(this string pName)
        {
            string displayName;

            // search in: CurrentUser
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey != null)
                    {
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                        {
                            var version = subkey.GetValue("DisplayVersion") as string;
                            if (version.IsNullOrEmpty() || version == null)
                            {
                                return null;
                            }

                            return new Version(version);
                        }
                    }
                }
            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey != null)
                    {
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                        {
                            var version = subkey.GetValue("DisplayVersion") as string;
                            if (version.IsNullOrEmpty() || version == null)
                            {
                                return null;
                            }

                            return new Version(version);
                        }
                    }
                }
            }

            // search in: LocalMachine_64
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey != null)
                    {
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                        {
                            var version = subkey.GetValue("DisplayVersion") as string;
                            if (version.IsNullOrEmpty() || version == null)
                            {
                                return null;
                            }

                            return new Version(version);
                        }
                    }
                }
            }

            // NOT FOUND
            return null;
        }
    }
}
