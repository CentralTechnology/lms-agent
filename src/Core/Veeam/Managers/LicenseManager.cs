namespace LMS.Core.Veeam.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Abp;
    using Core.Managers;
    using Extensions;
    using Microsoft.Win32;

    public class LicenseManager : LMSManagerBase, ILicenseManager
    {
        private Dictionary<string, string> _lic;
        public string LicenseFile { get; private set; }


        public  Dictionary<string, string> ExtractPropertiesFromLicense()
        {
            string[] licenseArray = LicenseFile.Split(new[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            return licenseArray.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
        }

        public string GetProperty(string name)
        {
            bool exists = _lic.TryGetValue(name, out string value);
            if (exists)
            {
                return value;
            }

            throw new AbpException($"Property: {name}, cannot be found in the Veeam license file. Make sure you have typed the property in correctly.");
        }

        public TResult GetProperty<TResult>(string name)
            where TResult : struct
        {
            return GetProperty(name).To<TResult>();
        }

        public string GetPropertyNoThrow(string name)
        {
            try
            {
                return GetProperty(name);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public TResult GetPropertyNoThrow<TResult>(string name)
            where TResult : struct
        {
            try
            {
                return GetProperty(name).To<TResult>();
            }
            catch (Exception)
            {
                return default(TResult);
            }
        }

        public string LoadFromRegistry()
        {
            try
            {
                RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Veeam\Veeam Backup and Replication");
                if (key == null)
                {
                    throw new AbpException("Unable to locate the Veeam registry hive. Please make sure Veeam is installed correctly.");
                }

                var license = key.GetSearchValue<byte[]>("license", "Lic1");

                if (license == null)
                {
                    throw new AbpException("Unable to retrieve the Veeam license from the registry. Please make sure Veeam is installed correctly.");
                }

                return Encoding.UTF8.GetString(license);
            }
            catch (SecurityException ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
        }

        public void SetLicenseFile(string licenseFile = null)
        {
            if (licenseFile == null)
            {
                LicenseFile = LoadFromRegistry();
            }
            else
            {
                LicenseFile = licenseFile;
            }

            _lic = ExtractPropertiesFromLicense();
        }
    }
}