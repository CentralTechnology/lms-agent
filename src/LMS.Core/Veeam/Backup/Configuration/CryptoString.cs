using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration
{
    public class CryptoString
    {
        public string Crypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return Convert.ToBase64String(DataProtection.Protect(Encoding.Unicode.GetBytes(value)));
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return new string(Encoding.Unicode.GetChars(DataProtection.Unprotect(Convert.FromBase64String(value))));
        }
    }
}
