using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Common
{
    public interface IOptionsProvider
    {
        bool HasValue(string valueName);

        object GetValue(string valueName);

        object GetValue(string valueName, object defaultValue);

        object GetValue(string valueName, object defaultValue, RegistryValueOptions options);
    }
}
