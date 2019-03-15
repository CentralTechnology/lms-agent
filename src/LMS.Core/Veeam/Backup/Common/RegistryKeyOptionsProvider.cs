using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Common
{
    public sealed class RegistryKeyOptionsProvider : IOptionsProvider
    {
        private readonly object _valuesLock = new object();
        private readonly RegistryKey _key;
        private HashSet<string> _values;

        public RegistryKeyOptionsProvider(RegistryKey key)
        {
            this._key = key;
        }

        public object GetValue(string valueName)
        {
            return this._key.GetValue(valueName);
        }

        public object GetValue(string valueName, object defaultValue)
        {
            return this._key.GetValue(valueName, defaultValue);
        }

        public object GetValue(string valueName, object defaultValue, RegistryValueOptions options)
        {
            return this._key.GetValue(valueName, defaultValue, options);
        }

        public bool HasValue(string valueName)
        {
            if (this._values == null)
            {
                lock (this._valuesLock)
                {
                    if (this._values == null)
                        this._values = new HashSet<string>((IEnumerable<string>) this._key.GetValueNames(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                }
            }
            return this._values.Contains(valueName);
        }
    }
}
