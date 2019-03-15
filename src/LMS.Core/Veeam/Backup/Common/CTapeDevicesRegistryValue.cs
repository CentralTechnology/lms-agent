using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public class CTapeDevicesRegistryValue : ITapeDevicesRegistryValue
  {
    private List<CTapeProxyRegInfo> _proxiesMatchInfo;

    public CTapeDevicesRegistryValue()
    {
      this._proxiesMatchInfo = new List<CTapeProxyRegInfo>();
    }

    public bool IsManualMatchSetByUser
    {
      get
      {
        return this._proxiesMatchInfo.Any<CTapeProxyRegInfo>((Func<CTapeProxyRegInfo, bool>) (x => x.ContainsAnyChanger));
      }
    }

    public bool ContainsMatching(string tapeServerName, string changerSysName)
    {
      CTapeProxyRegInfo ctapeProxyRegInfo = this._proxiesMatchInfo.FirstOrDefault<CTapeProxyRegInfo>((Func<CTapeProxyRegInfo, bool>) (x => string.Equals(x.ProxyName, tapeServerName, StringComparison.CurrentCultureIgnoreCase)));
      if (ctapeProxyRegInfo == null)
        return false;
      return ctapeProxyRegInfo.ContainsChanger(changerSysName);
    }

    public IEnumerable<CTapeDriveRegValue> GetDrives(
      string tapeServerName,
      string changerName)
    {
      return this._proxiesMatchInfo.Where<CTapeProxyRegInfo>((Func<CTapeProxyRegInfo, bool>) (p => string.Equals(p.ProxyName, tapeServerName, StringComparison.CurrentCultureIgnoreCase))).SelectMany((Func<CTapeProxyRegInfo, IEnumerable<CTapeChangerRegValue>>) (p => (IEnumerable<CTapeChangerRegValue>) p.Changers), (p, c) => new
      {
        p = p,
        c = c
      }).Where(_param1 => string.Equals(_param1.c.ChangerSysName, changerName, StringComparison.CurrentCultureIgnoreCase)).SelectMany(_param0 => (IEnumerable<CTapeDriveRegValue>) _param0.c.Drives, (_param0, d) => d);
    }

    public void Deserialize(string[] registryKeyValue)
    {
      try
      {
        this.DeserializeInner(registryKeyValue);
      }
      catch (Exception ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string str in registryKeyValue)
          stringBuilder.AppendLine(str);
        Log.Error(ex, "Error during deserializing registry key TapeDevices. KeyValue:{0}{1}{0}", (object) Environment.NewLine, (object) stringBuilder);
      }
    }

    private void DeserializeInner(string[] registryKeyValue)
    {
      string[] strArray1 = registryKeyValue;
      CTapeProxyRegInfo proxy = new CTapeProxyRegInfo();
      foreach (string str1 in strArray1)
      {
        string str2 = str1.Trim();
        if (!string.IsNullOrEmpty(str2))
        {
          if (this.IsProxyNameLine(str2))
          {
            this.AddProxyIfNotEmpty(proxy);
            proxy = new CTapeProxyRegInfo(str2);
          }
          string[] strArray2 = str2.Split('=');
          if (strArray2.Length == 2)
          {
            CTapeChangerRegValue changer = new CTapeChangerRegValue(strArray2[0]);
            proxy.AddChanger(changer);
            string str3 = strArray2[1];
            char[] chArray1 = new char[1]{ ',' };
            foreach (string str4 in str3.Split(chArray1))
            {
              char[] chArray2 = new char[1]{ ':' };
              string[] strArray3 = str4.Split(chArray2);
              if (strArray3.Length == 2)
              {
                string driveSysName = strArray3[0];
                int result;
                if (!int.TryParse(strArray3[1], out result))
                  result = 0;
                CTapeDriveRegValue drive = new CTapeDriveRegValue(driveSysName, result);
                changer.AddDrive(drive);
              }
            }
          }
        }
      }
      this.AddProxyIfNotEmpty(proxy);
    }

    private void AddProxyIfNotEmpty(CTapeProxyRegInfo proxy)
    {
      if (!proxy.ContainsAnyChanger)
        return;
      this._proxiesMatchInfo.Add(proxy);
    }

    private bool IsProxyNameLine(string line)
    {
      return !line.StartsWith("Changer", StringComparison.OrdinalIgnoreCase);
    }
  }
}
