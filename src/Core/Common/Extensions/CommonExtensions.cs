namespace Core.Common.Extensions
{
    using System;
    using Abp.Extensions;
    using Microsoft.Win32;

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

            return null;
        }

        private static (bool exist, string value) GetSubKeyValue(this RegistryKey key, string[] subKeyNames, string pName, string requestValue = null)
        {
            foreach (string keyName in subKeyNames)
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                if (subkey != null)
                {
                    string displayName = subkey.GetValue("DisplayName") as string;
                    if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (requestValue == null)
                        {
                            return (true, string.Empty);
                        }

                        string value = subkey.GetValue(requestValue) as string;
                        if (value.IsNullOrEmpty() || value == null)
                        {
                            return (false, string.Empty);
                        }

                        return (true, value);
                    }
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
    }
}