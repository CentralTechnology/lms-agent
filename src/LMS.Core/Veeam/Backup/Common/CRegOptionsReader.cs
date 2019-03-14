using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Common;
using Microsoft.Win32;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public class CRegOptionsReader : IOptionsReader
  {
    private static readonly object IsNotExistsObject = new object();
    private readonly IOptionsProvider _regKey;

    public static CRegOptionsReader New(RegistryKey optionsRegKey)
    {
      return CRegOptionsReader.New((IOptionsProvider) new RegistryKeyOptionsProvider(optionsRegKey));
    }

    public static CRegOptionsReader New(IOptionsProvider optionsProvider)
    {
      return new CRegOptionsReader(optionsProvider);
    }

    private CRegOptionsReader(IOptionsProvider optionsProvider)
    {
      if (optionsProvider == null)
        throw new ArgumentNullException(nameof (optionsProvider));
      this._regKey = optionsProvider;
    }

    public string GetString(string regValueName)
    {
      try
      {
        if (!this.IsOptionSpecified(regValueName))
          throw new CAppException("Application option [{0}] is not specified.", new object[1]
          {
            (object) regValueName
          });
        return Convert.ToString(this._regKey.GetValue(regValueName));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve mandatory application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public string GetOptionalString(string regValueName, string defaultValue)
    {
      try
      {
        return Convert.ToString(this._regKey.GetValue(regValueName, (object) defaultValue));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public string GetOptionalString(
      string regValueName,
      string defaultValue,
      RegistryValueOptions options)
    {
      try
      {
        return Convert.ToString(this._regKey.GetValue(regValueName, (object) defaultValue, options));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public string[] GetOptionalMultiString(string regValueName, string[] defaultValue)
    {
      try
      {
        return this._regKey.GetValue(regValueName, (object) defaultValue) as string[] ?? defaultValue;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public bool IsOptionSpecified(string regOptionName)
    {
      return this._regKey.HasValue(regOptionName);
    }

    public int GetInt32(string regValueName)
    {
      try
      {
        if (!this.IsOptionSpecified(regValueName))
          throw new CAppException("Application option {0} is not specified", new object[1]
          {
            (object) regValueName
          });
        return Convert.ToInt32(this._regKey.GetValue(regValueName));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to find optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public int GetOptionalInt32(string regValueName, int defaultValue)
    {
      try
      {
        if (this._regKey == null)
          return defaultValue;
        return Convert.ToInt32(this._regKey.GetValue(regValueName, (object) defaultValue));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public bool TryGetOptionalValue(string optionName, out object obj)
    {
      try
      {
        if (this._regKey == null)
        {
          obj = (object) null;
          return false;
        }
        object objA = this._regKey.GetValue(optionName, CRegOptionsReader.IsNotExistsObject);
        if (object.ReferenceEquals(objA, CRegOptionsReader.IsNotExistsObject))
        {
          obj = (object) null;
          return false;
        }
        obj = objA;
        return true;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) optionName);
        throw;
      }
    }

    public T GetOptionalInt32Enum<T>(string regValueName, T defaultValue) where T : struct
    {
      if (this._regKey == null)
        return defaultValue;
      object objA = this._regKey.GetValue(regValueName);
      if (object.ReferenceEquals(objA, (object) null))
        return defaultValue;
      int int32 = Convert.ToInt32(objA);
      T obj = Caster<int, T>.Cast(int32);
      if (EnumCache<T>.IsDefined(obj))
        return obj;
      Log.Warning("An enum value [{0}] with the name [{1}] is out of list [{2}]", (object) int32, (object) regValueName, (object) string.Join<int>(", ", ((IEnumerable<T>) EnumCache<T>.Values).Select<T, int>(Caster<T, int>.Cast)));
      return defaultValue;
    }

    public long GetOptionalInt64(string regValueName, long defaultValue)
    {
      try
      {
        if (this._regKey == null)
          return defaultValue;
        return Convert.ToInt64(this._regKey.GetValue(regValueName, (object) defaultValue));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public ulong GetOptionalUInt64(string regValueName, ulong defaultValue)
    {
      try
      {
        if (this._regKey == null)
          return defaultValue;
        return Convert.ToUInt64(this._regKey.GetValue(regValueName, (object) defaultValue));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public ulong GetOptionalBytesFromMb(string regValueName, ulong defaultBytes)
    {
      try
      {
        if (this._regKey == null)
          return defaultBytes;
        return Convert.ToUInt64(this._regKey.GetValue(regValueName, (object) (defaultBytes / 1024UL / 1024UL))) * 1024UL * 1024UL;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public double GetOptionalInt32AsDouble(
      string regValueName,
      double divider,
      double defaultValue)
    {
      try
      {
        if (this._regKey == null)
          return defaultValue;
        return Convert.ToDouble(this._regKey.GetValue(regValueName, (object) (defaultValue * divider))) / divider;
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public bool GetOptionalBoolean(string regValueName, bool defaultValue)
    {
      try
      {
        if (this._regKey == null)
          return defaultValue;
        return Convert.ToBoolean(this._regKey.GetValue(regValueName, (object) defaultValue));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regValueName);
        throw;
      }
    }

    public void ReadOptionalBoolean(string regName, ref bool result)
    {
      try
      {
        result = Convert.ToBoolean(this._regKey.GetValue(regName, (object) result));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regName);
        throw;
      }
    }

    public void ReadOptionalString(string regName, ref string result)
    {
      try
      {
        result = Convert.ToString(this._regKey.GetValue(regName, (object) result));
      }
      catch (Exception ex)
      {
        CExceptionUtil.RegenTraceExc(ex, "Failed to retrieve optional application option: [{0}]", (object) regName);
        throw;
      }
    }

    public bool KeyExists(string regName)
    {
      if (this._regKey == null)
        return false;
      return this._regKey.GetValue(regName, (object) null) != null;
    }
  }
}
