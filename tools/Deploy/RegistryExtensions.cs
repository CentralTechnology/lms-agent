namespace Deploy
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

    public class Register
    {
        public bool CheckInstalled(string cName,out string kName)
        {
            string displayName;
            
            string registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    kName = subkey.ToString();
                    if (displayName != null && displayName.Contains(cName))
                    {
                        
                        return true;
                    }
                }
                key.Close();
            }

            registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {                    
                    displayName = subkey.GetValue("DisplayName") as string;
                    kName = subkey.ToString();
                    if (displayName != null && displayName.Contains(cName))
                    {
                        kName = subkey.ToString();
                        return true;
                    }
                }
                key.Close();
            }
            kName = null;
            return false;
        }
    }
}