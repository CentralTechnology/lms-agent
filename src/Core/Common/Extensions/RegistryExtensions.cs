namespace LMS.Common.Extensions
{
    using Abp;
    using Microsoft.Win32;

    public static class RegistryExtentions
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
    }
}