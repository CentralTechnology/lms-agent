namespace Core.Common.Extensions
{
    using System;
    using Abp.Extensions;
    using Microsoft.Win32;
    using System.Linq;

    public static class CommonExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        private static bool DisplayNameExists(this RegistryKey key, string pName)
        {
            var data = key.GetSubKeyValue(key.GetSubKeyNames(), pName);

            return data.exist;
        }

        /// <summary>
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static Version GetApplicationVersion(this string pName)
        {
            var keys = new []
            {
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            };

            foreach (RegistryKey key in keys)
            {
                if (key != null)
                {
                    (bool exist, string value) data = key.GetSubKeyValue(key.GetSubKeyNames(), pName, "DisplayVersion");
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
                    var data = key.GetSubKeyValue(key.GetSubKeyNames(), pName, "DeviceID");
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

        private static (bool exist, string value) GetSubKeyValue(this RegistryKey key, string[] subKeyNames, string pName, string requestValue = null)
        {
            if (requestValue.IsNullOrEmpty())
            {
                requestValue = "DisplayName";
            }

            foreach (string keyName in subKeyNames.Where(skn => skn.Equals(pName, StringComparison.OrdinalIgnoreCase)))
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                if (subkey != null)
                {
                    string value = subkey.GetValue(requestValue) as string;
                    if (value.IsNullOrEmpty() || value == null)
                    {
                        return (false, string.Empty);
                    }

                    return (true, value);
                }
            }

            return (false, string.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static bool IsApplictionInstalled(this string pName)
        {
            var keys = new[]
            {
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            };

            foreach (var key in keys)
            {
                if (key != null)
                {
                    var data = key.GetSubKeyValue(key.GetSubKeyNames(), pName);
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