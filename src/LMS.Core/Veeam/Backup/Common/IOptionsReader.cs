using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Common
{
    public interface IOptionsReader
    {
        string GetString(string regValueName);

        string GetOptionalString(string regValueName, string defaultValue);

        string GetOptionalString(
            string regValueName,
            string defaultValue,
            RegistryValueOptions options);

        string[] GetOptionalMultiString(string regValueName, string[] defaultValue);

        bool IsOptionSpecified(string regOptionName);

        int GetInt32(string regValueName);

        int GetOptionalInt32(string regValueName, int defaultValue);

        T GetOptionalInt32Enum<T>(string regValueName, T defaultValue) where T : struct;

        long GetOptionalInt64(string regValueName, long defaultValue);

        ulong GetOptionalUInt64(string regValueName, ulong defaultValue);

        ulong GetOptionalBytesFromMb(string regValueName, ulong defaultBytes);

        double GetOptionalInt32AsDouble(string regValueName, double divider, double defaultValue);

        bool GetOptionalBoolean(string regValueName, bool defaultValue);

        void ReadOptionalBoolean(string regName, ref bool result);

        void ReadOptionalString(string regName, ref string result);

        bool KeyExists(string regName);

        bool TryGetOptionalValue(string optionName, out object obj);
    }
}
