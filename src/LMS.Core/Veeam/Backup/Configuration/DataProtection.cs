using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration
{
    internal class DataProtection
    {
        private const uint NTE_BAD_KEY_STATE = 2148073483;

        public static byte[] Protect(byte[] value)
        {
            return ProtectedData.Protect(value, (byte[]) null, DataProtectionScope.LocalMachine);
        }

        public static byte[] Unprotect(byte[] value)
        {
            try
            {
                return ProtectedData.Unprotect(value, (byte[]) null, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException ex)
            {
                if (ex.HResult == -2146893813)
                    throw new CorruptedConfigurationException("Configuration database is corrupted.", new object[0]);
                throw;
            }
        }
    }
}
