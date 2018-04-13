namespace LMS.Core.Extensions
{
    using System;
    using System.Linq;
    using Abp;
    using Abp.Extensions;
    using Microsoft.Win32;

    public static class RegistryKeyExtensions
    {
        public static TResult GetSearchValue<TResult>(this RegistryKey key, string searchName, string searchValue)
            where TResult : class
        {
            foreach (string keyName in key.GetSubKeyNames())
            {
                if (keyName == searchName)
                {
                    RegistryKey subKey = key.OpenSubKey(keyName);
                    if (subKey == null)
                    {
                        throw new AbpException($"Unable to open subkey registry entry for {keyName}");
                    }

                    return subKey.GetValue(searchValue) as TResult;
                }
            }

            return null;
        }

        public static (bool exist, string value) GetSubKeyValue(this RegistryKey key, string[] subKeyNames, NameValue filterBy = null, string requestedKeyName = null, string requestedValue = null)
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
                    if (subkey != null)
                    {
                        if (subkey.GetValue(requestedValue) is string value && !value.IsNullOrEmpty())
                        {
                            return (true, value);
                        }
                    }
                }
            }

            return (false, string.Empty);
        }
    }
}