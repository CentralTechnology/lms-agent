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
            // search in: CurrentUser
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                if (key.DisplayNameExists(pName))
                {
                    return true;
                }
            }

            // search in: LocalMachine_32
            key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                if (key.DisplayNameExists(pName))
                {
                    return true;
                }
            }

            // search in: LocalMachine_64
            key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                if (key.DisplayNameExists(pName))
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
        /// <param name="pName"></param>
        /// <returns></returns>
        private static bool DisplayNameExists(this RegistryKey key, string pName)
        {
            foreach (string keyName in key.GetSubKeyNames())
            {
                RegistryKey subKey = key.OpenSubKey(keyName);
                if (subKey != null)
                {
                    var displayName =  subKey.GetValue("DisplayName") as string;
                    if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static Guid? GetCentrastageId(this string pName)
        {
#if DEBUG
            return new Guid("2a5d23dc-1b9a-9341-32c6-1160a5df7883");
#endif
            string deviceId;

            // search in: LocalMachine_32
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE");
            if (key != null)
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    RegistryKey subKey = key.OpenSubKey(keyName);
                    if (subKey != null)
                    {
                        deviceId = subKey.GetValue("DeviceID") as string;
                        if (!deviceId.IsNullOrEmpty() || deviceId != null)
                        {
                            bool valid = Guid.TryParse(deviceId, out Guid csId);
                            if (valid)
                            {
                                return csId;
                            }
                        }
                    }
                }
            }

            // search in: LocalMachine_64
            key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE");
            if (key != null)
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    RegistryKey subKey = key.OpenSubKey(keyName);
                    if (subKey != null)
                    {
                        deviceId = subKey.GetValue("DeviceID") as string;
                        if (!deviceId.IsNullOrEmpty() || deviceId != null)
                        {
                            bool valid = Guid.TryParse(deviceId, out Guid csId);
                            if (valid)
                            {
                                return csId;
                            }
                        }
                    }
                }
            }

            return null;
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
