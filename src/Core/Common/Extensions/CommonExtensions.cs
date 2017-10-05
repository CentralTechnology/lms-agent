namespace Core.Common.Extensions
{
    using System;
    using Abp.Extensions;
    using Microsoft.Win32;
    using System.Linq;
    using Abp;
    using Models;
    using NLog;

    public static class CommonExtensions
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
        public static Version GetApplicationVersion(string pName)
        {
            foreach (RegistryKey key in UninstallKeys)
            {
                if (key != null)
                {
                    (bool exist, string value) data = key.GetSubKeyValue(key.GetSubKeyNames(), new NameValue("DisplayName", pName), requestedValue: "DisplayVersion");
                    if (data.exist)
                    {
                        return new Version(data.value);
                    }
                }
            }

            // NOT FOUND
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static Guid? GetCentrastageId(this string pName)
        {
#if DEBUG
            return new Guid("2a5d23dc-1b9a-9341-32c6-1160a5df7883");
#endif
            var keys = new[]
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE")
            };

            foreach (var key in keys)
            {
                try
                {
                    var data = key.GetSubKeyValue(key.GetSubKeyNames(), requestedKeyName: "CentraStage", requestedValue:"DeviceID");
                    if (data.exist)
                    {
                        bool valid = Guid.TryParse(data.value, out Guid csId);
                        if (valid)
                        {
                            return csId;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                }                
            }

            return null;
        }

        private static (bool exist, string value) GetSubKeyValue(this RegistryKey key, string[] subKeyNames, NameValue filterBy = null, string requestedKeyName = null, string requestedValue = null)
        {
            if (requestedValue.IsNullOrEmpty())
            {
                requestedValue = "DisplayName";
            }

            if (requestedKeyName == null)
            {
                foreach (string keyName in subKeyNames)
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey != null)
                    {
                        if (filterBy == null)
                        {
                            string value = subkey.GetValue(requestedValue) as string;
                            if (value != null)
                            {
                                return (true, value);
                            }
                        }
                        else
                        {
                            string filterByValue = subkey.GetValue(filterBy.Name) as string;
                            if (filterBy.Value.Equals(filterByValue, StringComparison.OrdinalIgnoreCase))
                            {
                                string value = subkey.GetValue(requestedValue) as string;
                                if (value != null)
                                {
                                    return (true, value);
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                foreach (string keyName in subKeyNames.Where(skn => skn.Equals(requestedKeyName, StringComparison.OrdinalIgnoreCase)))
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    if (subkey != null)
                    {
                        string value = subkey.GetValue(requestedValue) as string;
                        if (value != null && !value.IsNullOrEmpty())
                        {
                            return (true, value);
                        }                        
                    }
                }
            }

            return (false, string.Empty);
        }

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