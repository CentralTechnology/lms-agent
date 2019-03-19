using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public class RegistryConfigurationController : IRegistryConfigurationController, IDisposable
  {
    private readonly Lazy<CryptoString> _cryptoString = new Lazy<CryptoString>((Func<CryptoString>) (() => new CryptoString()));

    public string SubKey { get; private set; }

    public RegistryKey RegistryKey { get; private set; }

    private RegistryConfigurationController(string subKey, RegistryKey registryKey)
    {
      this.SubKey = subKey;
      this.RegistryKey = registryKey;
    }

    public bool IsReady
    {
      get
      {
        return this.RegistryKey != null;
      }
    }

    public T GetValue<T>(string name, T defaultValue) where T : class
    {
      if (this.RegistryKey != null)
        return this.RegistryKey.GetValue(name, (object) defaultValue) as T;
      return defaultValue;
    }

    public string GetValue(string name, string defaultValue, RegistryControllerValueOption option)
    {
      if (this.RegistryKey == null)
        return defaultValue;
      switch (option)
      {
        case RegistryControllerValueOption.String:
          return Convert.ToString(this.RegistryKey.GetValue(name, (object) defaultValue, RegistryValueOptions.None));
        case RegistryControllerValueOption.ExpandString:
          return Convert.ToString(this.RegistryKey.GetValue(name, (object) defaultValue, RegistryValueOptions.DoNotExpandEnvironmentNames));
        case RegistryControllerValueOption.DecryptString:
          object obj = this.RegistryKey.GetValue(name);
          if (obj != null)
            return this._cryptoString.Value.Decrypt(Convert.ToString(obj));
          return defaultValue;
        default:
          throw new InvalidEnumArgumentException(nameof (option), (int) option, typeof (RegistryControllerValueOption));
      }
    }

    public int GetValue(string name, int defaultValue)
    {
      if (this.RegistryKey != null)
        return Convert.ToInt32(this.RegistryKey.GetValue(name, (object) defaultValue));
      return defaultValue;
    }

    public long GetValue(string name, long defaultValue)
    {
      if (this.RegistryKey != null)
        return Convert.ToInt64(this.RegistryKey.GetValue(name, (object) defaultValue));
      return defaultValue;
    }

    public bool GetValue(string name, bool defaultValue)
    {
      if (this.RegistryKey != null)
        return Convert.ToBoolean(this.RegistryKey.GetValue(name, (object) defaultValue));
      return defaultValue;
    }

    public byte[] GetValue(string name, byte[] defaultValue)
    {
      if (this.RegistryKey == null)
        return defaultValue;
      return (byte[]) this.RegistryKey.GetValue(name, (object) defaultValue);
    }

    public bool ContainsValue(string name)
    {
      return this.RegistryKey != null && this.RegistryKey.GetValue(name) != null;
    }

    public bool? FindBooleanValue(string name)
    {
      if (this.RegistryKey == null)
        return new bool?();
      object obj = this.RegistryKey.GetValue(name);
      if (obj == null)
        return new bool?();
      return new bool?(Convert.ToBoolean(obj));
    }

    private RegistryKey OpenRegistryKey()
    {
      return this.RegistryKey ?? (this.RegistryKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(this.SubKey, RegistryKeyPermissionCheck.ReadWriteSubTree));
    }

    public void SetValue(string name, string value, RegistryControllerValueOption option)
    {
      value = string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
      switch (option)
      {
        case RegistryControllerValueOption.String:
          this.OpenRegistryKey().SetValue(name, (object) value, RegistryValueKind.String);
          break;
        case RegistryControllerValueOption.ExpandString:
          this.OpenRegistryKey().SetValue(name, (object) value, RegistryValueKind.ExpandString);
          break;
        case RegistryControllerValueOption.CryptString:
          this.OpenRegistryKey().SetValue(name, (object) this._cryptoString.Value.Crypt(value), RegistryValueKind.String);
          break;
        default:
          throw new InvalidEnumArgumentException(nameof (option), (int) option, typeof (RegistryControllerValueOption));
      }
    }

    public void SetValue(
      string name,
      string value,
      string defaultValue,
      RegistryControllerValueOption option)
    {
      bool flag;
      switch (option)
      {
        case RegistryControllerValueOption.String:
        case RegistryControllerValueOption.ExpandString:
          flag = value != this.GetValue(name, defaultValue, option);
          break;
        case RegistryControllerValueOption.CryptString:
          flag = value != this.GetValue(name, defaultValue, RegistryControllerValueOption.DecryptString);
          break;
        default:
          throw new InvalidEnumArgumentException(nameof (option), (int) option, typeof (RegistryControllerValueOption));
      }
      if (!flag)
        return;
      this.SetValue(name, value, option);
    }

    public void SetValue(string name, int value)
    {
      this.OpenRegistryKey().SetValue(name, (object) value, RegistryValueKind.DWord);
    }

    public void SetValue(string name, int value, int defaultValue)
    {
      if (value == this.GetValue(name, defaultValue))
        return;
      this.SetValue(name, value);
    }

    public void SetValue(string name, bool value)
    {
      this.OpenRegistryKey().SetValue(name, (object) value, RegistryValueKind.DWord);
    }

    public void SetValue(string name, string[] value)
    {
      this.OpenRegistryKey().SetValue(name, (object) value, RegistryValueKind.MultiString);
    }

    public void SetValue(string name, bool value, bool defaultValue)
    {
      if (value == this.GetValue(name, defaultValue))
        return;
      this.SetValue(name, value);
    }

    public void SetValue(string name, byte[] value)
    {
      this.OpenRegistryKey().SetValue(name, (object) value, RegistryValueKind.Binary);
    }

    public void DeleteValue(string name)
    {
      this.OpenRegistryKey().DeleteValue(name);
    }

    IRegistryConfigurationController IRegistryConfigurationController.Open(
      string subKey,
      bool writable,
      RegistryView registryView)
    {
      string str = string.Format("{0}\\{1}", (object) this.SubKey, (object) subKey);
      RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(str, writable);
      return (IRegistryConfigurationController) new RegistryConfigurationController(str, registryKey);
    }

    public void Dispose()
    {
      if (this.RegistryKey == null)
        return;
      this.RegistryKey.Dispose();
      this.RegistryKey = (RegistryKey) null;
    }

    public void SetAccessControl(
      SecurityIdentifier owner,
      IEnumerable<SecurityIdentifier> identities)
    {
      if (identities == null)
        throw new ArgumentNullException(nameof (identities));
      new RegistryConfigurationAccessController(this.OpenRegistryKey())
      {
        Owner = owner,
        Identities = identities.ToArray<SecurityIdentifier>()
      }.Set();
    }

    public static IRegistryConfigurationController Create(
      string subKey,
      bool writable,
      RegistryView registryView = RegistryView.Default)
    {
      return (IRegistryConfigurationController) new RegistryConfigurationController(subKey, RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).CreateSubKey(subKey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree));
    }

    public static IRegistryConfigurationController Open(
      string subKey,
      bool writable,
      RegistryView registryView = RegistryView.Default)
    {
      return (IRegistryConfigurationController) new RegistryConfigurationController(subKey, RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(subKey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree));
    }
  }
}
