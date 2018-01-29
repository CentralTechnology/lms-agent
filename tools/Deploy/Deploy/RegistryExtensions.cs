﻿namespace Deploy
{
    using System;
    using System.Linq;
    using Microsoft.Win32;

    public static class RegistryExtensions
    {
        public static (bool exist, string value) GetSubKeyValue(this RegistryKey key, string[] subKeyNames, NameValue filterBy = null, string requestedKeyName = null, string requestedValue = null)
        {
            if (string.IsNullOrEmpty(requestedValue))
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
                            if (subkey.GetValue(requestedValue) is string value)
                            {
                                return (true, value);
                            }
                        }
                        else
                        {
                            string filterByValue = subkey.GetValue(filterBy.Name) as string;
                            if (filterBy.Value.Equals(filterByValue, StringComparison.OrdinalIgnoreCase))
                            {
                                if (subkey.GetValue(requestedValue) is string value)
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
                    if (subkey?.GetValue(requestedValue) is string value && !string.IsNullOrEmpty(value))
                    {
                        return (true, value);
                    }
                }
            }

            return (false, string.Empty);
        }
    }
}