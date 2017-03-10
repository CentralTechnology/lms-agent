namespace Core.Common.Extensions
{
    using System;
    using Microsoft.Win32;

    public static class RegistryExtentions
    {
        /// <summary>
        ///     gets the registry key with a given path
        /// </summary>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        private static RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                    Environment.Is64BitOperatingSystem
                        ? RegistryView.Registry64
                        : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }

        /// <summary>
        ///     gets the registry value from the key path and name
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static object GetRegistryValue(string keyPath, string keyName)
        {
            RegistryKey registry = GetRegistryKey(keyPath);
            return registry.GetValue(keyName);
        }
    }
}