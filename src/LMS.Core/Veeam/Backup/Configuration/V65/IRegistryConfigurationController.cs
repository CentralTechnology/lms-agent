using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IRegistryConfigurationController : IDisposable
    {
        string SubKey { get; }

        RegistryKey RegistryKey { get; }

        bool IsReady { get; }

        T GetValue<T>(string name, T defaultValue) where T : class;

        string GetValue(string name, string defaultValue, RegistryControllerValueOption option = RegistryControllerValueOption.String);

        int GetValue(string name, int defaultValue);

        long GetValue(string name, long defaultValue);

        bool GetValue(string name, bool defaultValue);

        byte[] GetValue(string name, byte[] defaultValue);

        bool? FindBooleanValue(string name);

        void SetValue(string name, string value, RegistryControllerValueOption option = RegistryControllerValueOption.String);

        void SetValue(
            string name,
            string value,
            string defaultValue,
            RegistryControllerValueOption option = RegistryControllerValueOption.String);

        void SetValue(string name, int value);

        void SetValue(string name, int value, int defaultValue);

        void SetValue(string name, bool value);

        void SetValue(string name, bool value, bool defaultValue);

        void SetValue(string name, byte[] value);

        void SetValue(string name, string[] value);

        void DeleteValue(string name);

        IRegistryConfigurationController Open(
            string subKey,
            bool writable,
            RegistryView registryView = RegistryView.Default);

        void SetAccessControl(SecurityIdentifier owner, IEnumerable<SecurityIdentifier> identities);

        bool ContainsValue(string name);
    }
}
