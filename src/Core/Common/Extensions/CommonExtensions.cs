namespace LMS.Common.Extensions
{
    using System;
    using System.Linq;
    using Abp;
    using Abp.Extensions;
    using Microsoft.Win32;

    public static class CommonExtensions
    {


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
    }
}