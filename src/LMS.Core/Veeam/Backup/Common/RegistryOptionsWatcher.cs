using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Configuration.V65;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Common
{
    using Serilog;

    public sealed class RegistryOptionsWatcher : IDisposable
  {
    private static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;
    private static readonly HashSet<string> _noLog = new HashSet<string>((IEqualityComparer<string>) RegistryOptionsWatcher.NameComparer)
    {
      "BackupServerPort",
      "CloudSvcPort",
      "SecureConnectionsPort",
      "SqlDatabaseName",
      "SqlInstanceName",
      "SqlLogin",
      "SqlServerName"
    };
    private readonly object _lock = new object();
    private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>((IEqualityComparer<string>) RegistryOptionsWatcher.NameComparer);
    private readonly HashSet<string> _previousInvalidNames = new HashSet<string>((IEqualityComparer<string>) RegistryOptionsWatcher.NameComparer);
    private readonly RegistryWatcher _watcher;
    private readonly ILogger _logger = Log.ForContext<RegistryOptionsWatcher>();
    private long _pendingChanges;

    public long Version { get; private set; }

    public RegistryOptionsWatcher(RegistryKey clonableKey)
    {
      if (clonableKey == null)
        throw new ArgumentNullException(nameof (clonableKey));
      try
      {
        object obj = clonableKey.GetValue("DisableRegistryWatching");
        if (obj is int && (int) obj == 1)
        {
          this._logger.Information("[Options] Cannot start a registry watching because it disabled by key [DisableRegistryWatching].");
          this.Failover();
        }
        else
        {
          this.Initialize(clonableKey);
          this._watcher = new RegistryWatcher(clonableKey, false, false, true, false, false);
          this._watcher.Changed += new Action<RegistryKey>(this.OnChanged);
          this._watcher.Removed += new Action<string>(this.OnRemoved);
          this._watcher.Terminated += new Action<Exception>(this.OnTerminated);
          this._watcher.Start();
        }
      }
      catch (Exception ex)
      {
        this._logger.Error(ex, "[Options] Cannot start a registry watching.");
        this.Failover();
      }
    }

    public void Dispose()
    {
      if (this._watcher != null)
        this._watcher.Dispose();
    }

    public IOptionsReader GetReader(RegistryKey failoverKey)
    {
      return (IOptionsReader) CRegOptionsReader.New(this._watcher == null || !this._watcher.IsWatching ? (IOptionsProvider) new RegistryKeyOptionsProvider(failoverKey) : (IOptionsProvider) new RegistryOptionsWatcher.OptionsProvider(this._values));
    }

    private void Initialize(RegistryKey key)
    {
      try
      {
        HashSet<string> invalidNames = (HashSet<string>) null;
        StringBuilder self = new StringBuilder();
        foreach (string valueName in key.GetValueNames())
        {
          string index = this.ProcessName(valueName, ref invalidNames);
          object obj1 = key.GetValue(valueName);
          if (this._values.TryAdd(index, obj1))
          {
            if (RegistryOptionsWatcher.CanLog(index))
              self.AppendFormatLine("\t{0}: {1}", (object) index, (object) RegistryOptionsWatcher.FormatValue(obj1));
          }
          else if (index == valueName)
          {
            object obj2;
            if (this._values.TryGetValue(index, out obj2))
            {
              if (RegistryOptionsWatcher.CanLog(index))
                self.AppendFormatLine("\t{0}: {1} /* Warning: option with the same name overwritten: {2} */", (object) index, (object) RegistryOptionsWatcher.FormatValue(obj1), (object) RegistryOptionsWatcher.FormatValue(obj2));
            }
            else if (RegistryOptionsWatcher.CanLog(index))
              self.AppendFormatLine("\t{0}: {1} /* Warning: option with the same name invalid */", (object) index, (object) RegistryOptionsWatcher.FormatValue(obj1));
            this._values[index] = obj1;
          }
          else
          {
            object obj2;
            if (this._values.TryGetValue(index, out obj2))
            {
              if (RegistryOptionsWatcher.CanLog(index))
                self.AppendFormatLine("\t{0}: {1} /* Warning: there is option with the same name, use old value: {2} */", (object) index, (object) RegistryOptionsWatcher.FormatValue(obj1), (object) RegistryOptionsWatcher.FormatValue(obj2));
            }
            else if (RegistryOptionsWatcher.CanLog(index))
              self.AppendFormatLine("\t{0}: {1} /* Warning: there is option with the same name, use old value */", (object) index, (object) RegistryOptionsWatcher.FormatValue(obj1));
          }
        }
        this.FillInvalidNames(invalidNames);
        if (self.Length <= 0)
          return;
        this._logger.Information("[Options] The following options are loaded from the registry:");
        this._logger.Information(self.ToString());
        this._logger.Information("[Options]");
      }
      catch (Exception ex)
      {
        this._logger.Error(ex, "[Options] Cannot get registry options.");
        this.Failover();
      }
    }

    private void OnChanged(RegistryKey key)
    {
      try
      {
        if (Interlocked.Increment(ref this._pendingChanges) > 1L)
          return;
        lock (this._lock)
        {
          do
          {
            this.ProcessChanged(key);
            ++this.Version;
          }
          while (Interlocked.Exchange(ref this._pendingChanges, 0L) != 0L);
        }
        SOptions.Reset();
      }
      catch (Exception ex)
      {
        this._logger.Error(ex, "[Options] Cannot handle registry changes.");
        this.Failover();
      }
    }

    private void OnRemoved(string keyName)
    {
      try
      {
        Interlocked.Exchange(ref this._pendingChanges, 0L);
        lock (this._lock)
        {
          this.ClearValues();
          ++this.Version;
        }
        this._logger.Warning("[Options] The tracked registry key [{0}] has been removed. Options will use default values.", (object) keyName);
        SOptions.Reset();
      }
      catch (Exception ex)
      {
        this._logger.Error(ex, "[Options] Cannot handle registry changes.");
        this.Failover();
      }
    }

    private void OnTerminated(Exception ex)
    {
      this._logger.Error(ex, "[Options] Registry watcher was unexpectedly terminated.");
      this.Failover();
    }

    private void Failover()
    {
      this._logger.Warning("[Options] Failover to a regular options provider.");
      if (this._watcher != null)
        this._watcher.Dispose();
      this.ClearValues();
    }

    private void ClearValues()
    {
      this._values.Clear();
      this._previousInvalidNames.Clear();
    }

    private void ProcessChanged(RegistryKey key)
    {
      HashSet<string> knownNames = new HashSet<string>((IEnumerable<string>) this._values.Keys, (IEqualityComparer<string>) RegistryOptionsWatcher.NameComparer);
      HashSet<string> invalidNames = (HashSet<string>) null;
      Dictionary<string, object> newNames = (Dictionary<string, object>) null;
      foreach (string valueName in key.GetValueNames())
        this.ProcessValue(key, valueName, ref newNames, ref invalidNames, knownNames);
      this.LogAndRemoveOldValues(knownNames, newNames);
      this.LogNewValues(newNames);
      this.FillInvalidNames(invalidNames);
    }

    private void LogNewValues(Dictionary<string, object> newNames)
    {
      if (newNames == null)
        return;
      foreach (KeyValuePair<string, object> newName in newNames)
        this.ProcessNewValue(newName.Key, newName.Value);
    }

    private bool IsDefaultKeyName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return false;
      int num = name.IndexOf('#');
      int result;
      return num >= 0 && num + 1 < name.Length && (int.TryParse(name.Substring(num + 1), NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result) && result <= 10);
    }

    private void LogAndRemoveOldValues(
      HashSet<string> knownNames,
      Dictionary<string, object> newNames)
    {
      foreach (string knownName in knownNames)
      {
        object objA;
        bool flag = this._values.TryRemove(knownName, out objA);
        if (RegistryOptionsWatcher.CanLog(knownName))
        {
          if (flag)
          {
            string str = (string) null;
            if (newNames != null)
            {
              foreach (KeyValuePair<string, object> newName in newNames)
              {
                if (object.ReferenceEquals(objA, (object) null))
                {
                  if (object.ReferenceEquals(newName.Value, (object) null))
                  {
                    str = newName.Key;
                    newNames.Remove(str);
                    break;
                  }
                }
                else if (newName.Value.Equals(objA))
                {
                  str = newName.Key;
                  newNames.Remove(str);
                  break;
                }
              }
            }
            if (str == null)
              this._logger.Information("[Options] The option [{0}] has removed. Old value: {1}.", (object) knownName, (object) RegistryOptionsWatcher.FormatValue(objA));
            else if (this.IsDefaultKeyName(knownName))
              this.ProcessNewValue(str, objA);
            else
              this._logger.Information("[Options] The option [{0}] renamed to [{1}]. Value: {2}.", (object) knownName, (object) str, (object) RegistryOptionsWatcher.FormatValue(objA));
          }
          else
            this._logger.Error("[Options] Cannot find removed option [{0}] in the cache. Possible the race condition.", (object) knownName);
        }
      }
    }

    private void FillInvalidNames(HashSet<string> invalidNames)
    {
      this._previousInvalidNames.Clear();
      if (invalidNames == null)
        return;
      this._previousInvalidNames.AddRange<string>((IEnumerable<string>) invalidNames);
    }

    private void ProcessValue(
      RegistryKey key,
      string givenName,
      ref Dictionary<string, object> newNames,
      ref HashSet<string> invalidNames,
      HashSet<string> knownNames)
    {
      string index = this.ProcessName(givenName, ref invalidNames);
      object obj = key.GetValue(givenName);
      if (knownNames.Remove(index))
        this.ProcessChangedValue(index, obj);
      else if (this._values.TryAdd(index, obj))
      {
        if (newNames == null)
          newNames = new Dictionary<string, object>((IEqualityComparer<string>) RegistryOptionsWatcher.NameComparer);
        newNames[index] = obj;
      }
      else
        this.ProcessUnexpectedValue(index, obj);
    }

    private static bool CanLog(string name)
    {
      return !name.Contains("Password", StringComparison.OrdinalIgnoreCase) && !RegistryOptionsWatcher._noLog.Contains(name);
    }

    private string ProcessName(string givenName, ref HashSet<string> invalidNames)
    {
      string str = givenName.Trim();
      if (str.Length != givenName.Length)
      {
        if (invalidNames == null)
          invalidNames = new HashSet<string>((IEqualityComparer<string>) RegistryOptionsWatcher.NameComparer);
        invalidNames.Add(givenName);
        if (!this._previousInvalidNames.Contains(givenName))
          this._logger.Error("[Options] An invalid option name [{0}] is corrected to [{1}].", (object) givenName, (object) str);
      }
      return str;
    }

    private void ProcessNewValue(string name, object value)
    {
      if (!RegistryOptionsWatcher.CanLog(name) || this.IsDefaultKeyName(name))
        return;
      this._logger.Information("[Options] The new option [{0}] has appeared. Value: {1}.", (object) name, (object) RegistryOptionsWatcher.FormatValue(value));
    }

    private void ProcessChangedValue(string name, object value)
    {
      object obj;
      if (this._values.TryGetValue(name, out obj))
      {
        if (object.ReferenceEquals(value, (object) null))
        {
          if (object.ReferenceEquals(obj, (object) null))
            return;
          if (RegistryOptionsWatcher.CanLog(name))
            this._logger.Information("[Options] The option [{0}] has changed {1} -> {2}.", (object) name, (object) RegistryOptionsWatcher.FormatValue(obj), (object) RegistryOptionsWatcher.FormatValue((object) null));
          this._values[name] = (object) null;
        }
        else
        {
          if (value.Equals(obj) || RegistryOptionsWatcher.IsSequenceEqual(value, obj))
            return;
          if (RegistryOptionsWatcher.CanLog(name))
            this._logger.Information("[Options] The option [{0}] has changed {1} -> {2}.", (object) name, (object) RegistryOptionsWatcher.FormatValue(obj), (object) RegistryOptionsWatcher.FormatValue(value));
          this._values[name] = value;
        }
      }
      else
        this._logger.Error("[Options] Cannot find option [{0}] in the cache. Possible the race condition.", (object) name);
    }

    private static string FormatValue(object value)
    {
      if (object.ReferenceEquals(value, (object) null))
        return "<null>";
      string str1 = value as string;
      if (!object.ReferenceEquals((object) str1, (object) null))
      {
        if (str1 == string.Empty)
          return "<empty>";
        if (str1.Length > 256)
          str1 = str1.Substring(0, 256) + " ... ";
        return 34.ToString() + str1 + (object) '"';
      }
      byte[] inArray = value as byte[];
      if (!object.ReferenceEquals((object) inArray, (object) null))
        return Convert.ToBase64String(inArray);
      IEnumerable source = value as IEnumerable;
      if (object.ReferenceEquals((object) source, (object) null))
        return value.ToString();
      string str2 = string.Join(", ", source.Cast<object>().Select<object, string>(new Func<object, string>(RegistryOptionsWatcher.FormatValue)));
      if (str2 == string.Empty)
        return "<empty>";
      if (str2.Length > 256)
        str2 = str2.Substring(0, 256) + " ... ";
      return 91.ToString() + str2 + (object) ']';
    }

    private static bool IsSequenceEqual(object value, object previousValue)
    {
      IEnumerable source1 = value as IEnumerable;
      IEnumerable source2 = previousValue as IEnumerable;
      return !object.ReferenceEquals((object) source1, (object) null) && !object.ReferenceEquals((object) source2, (object) null) && source1.Cast<object>().SequenceEqual<object>(source2.Cast<object>());
    }

    private void ProcessUnexpectedValue(string name, object value)
    {
      object obj;
      if (this._values.TryGetValue(name, out obj))
      {
        if (RegistryOptionsWatcher.CanLog(name))
          this._logger.Error("[Options] The option [{0}] has changed from {1} -> {2}. Possible the race condition.", (object) name, (object) RegistryOptionsWatcher.FormatValue(obj), (object) RegistryOptionsWatcher.FormatValue(value));
      }
      else if (RegistryOptionsWatcher.CanLog(name))
        this._logger.Error("[Options] The new option [{0}] has appeared. Value: {1}. Possible the race condition.", (object) name, (object) RegistryOptionsWatcher.FormatValue(value));
      this._values[name] = value;
    }

    private sealed class OptionsProvider : IOptionsProvider
    {
      private readonly ConcurrentDictionary<string, object> _values;

      public OptionsProvider(ConcurrentDictionary<string, object> values)
      {
        this._values = values;
      }

      public bool HasValue(string valueName)
      {
        return this._values.ContainsKey(valueName);
      }

      public object GetValue(string valueName)
      {
        object obj;
        if (this._values.TryGetValue(valueName, out obj))
          return obj;
        return (object) null;
      }

      public object GetValue(string valueName, object defaultValue)
      {
        object obj;
        if (this._values.TryGetValue(valueName, out obj))
          return obj;
        return defaultValue;
      }

      public object GetValue(string valueName, object defaultValue, RegistryValueOptions options)
      {
        object obj;
        if (this._values.TryGetValue(valueName, out obj))
          return obj;
        return defaultValue;
      }
    }
  }
}
