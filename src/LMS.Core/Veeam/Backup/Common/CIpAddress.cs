using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Common;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class CIpAddress
  {
    public static readonly string Empty = "...";

    public static string[] ToStrings(this IEnumerable<IPAddress> addresses)
    {
      return addresses.Select<IPAddress, string>((Func<IPAddress, string>) (ip => ip.ToString())).ToArray<string>();
    }

    public static IPAddress[] FromStrings(IEnumerable<string> addresses)
    {
      return addresses.Select<string, IPAddress>((Func<string, IPAddress>) (ip => IPAddress.Parse(ip))).ToArray<IPAddress>();
    }

    public static IPAddress[] ResolveConnPoints(string srvName, bool isLocal)
    {
      string serverName = srvName;
      if (isLocal)
        serverName = "localhost";
      return CIpAddress.ResolveConnectionPoints(serverName);
    }

    public static IPAddress[] ResolveConnectionPoints(string serverName)
    {
      try
      {
        return CIpAddress.GetIpAddresses(serverName);
      }
      catch (Exception ex)
      {
        List<IPAddress> ipAddresses = new List<IPAddress>();
        if (CIpAddress.TryGetHostAddresses(serverName, ipAddresses) || CIpAddress.TryGetNetbiosIps(serverName, ipAddresses))
          return ipAddresses.ToArray();
        throw ExceptionFactory.Create(ex, "Failed to resolve host name {0} from {1}", (object) serverName, (object) Environment.MachineName);
      }
    }

    public static IPAddress[] ForceResolveConnectionPoints(string serverName)
    {
      return CIpAddress.ForceResolveConnectionPointsImpl(serverName, false);
    }

    private static IPAddress[] ForceResolveConnectionPointsImpl(
      string serverName,
      bool ignoreLoopback)
    {
      List<IPAddress> ipAddressList = new List<IPAddress>();
      ipAddressList.AddRange((IEnumerable<IPAddress>) CIpAddress.GetIpAddressesSafe(serverName));
      CIpAddress.TryGetHostAddresses(serverName, ipAddressList);
      CIpAddress.TryGetNetbiosIps(serverName, ipAddressList);
      IPAddress[] array = ipAddressList.Distinct<IPAddress>().Where<IPAddress>((Func<IPAddress, bool>) (ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToArray<IPAddress>();
      if (((IEnumerable<IPAddress>) array).Any<IPAddress>((Func<IPAddress, bool>) (r => r.Equals((object) IPAddress.Loopback))) && !ignoreLoopback)
      {
        ipAddressList.Clear();
        ipAddressList.AddRange((IEnumerable<IPAddress>) CIpAddress.ForceResolveConnectionPointsImpl(Environment.MachineName, true));
        ipAddressList.Add(IPAddress.Loopback);
        array = ipAddressList.Distinct<IPAddress>().Where<IPAddress>((Func<IPAddress, bool>) (ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToArray<IPAddress>();
      }
      return array;
    }

    public static IPAddress[] SafeResolveConnectionPoints(string serverName)
    {
      try
      {
        return CIpAddress.ResolveConnectionPoints(serverName);
      }
      catch
      {
        return new IPAddress[0];
      }
    }

    private static bool TryGetNetbiosIps(string serverName, List<IPAddress> ipAddresses)
    {
      try
      {
        int length = serverName.IndexOf('.');
        if (-1 == length)
          return false;
        return CIpAddress.TryGetHostAddresses(serverName.Substring(0, length), ipAddresses);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private static bool TryGetHostAddresses(string serverName, List<IPAddress> ipAddresses)
    {
      try
      {
        IPAddress[] hostAddresses = Dns.GetHostAddresses(serverName);
        if (hostAddresses.Length == 0)
          return false;
        ipAddresses.AddRange((IEnumerable<IPAddress>) hostAddresses);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool IsRoutable(string networkAddress, string mask)
    {
      return CIpAddress.ApplyMask(networkAddress, mask) == networkAddress;
    }

    public static uint ToNumber(string ip)
    {
      if (!CIpAddress.IsValidTemplated(ip))
        throw ExceptionFactory.Create("Wrong IP address {0}", (object) ip);
      return CIpAddress.ToNumberNoCheck(ip);
    }

    public static uint ToNumberNoCheck(string ip)
    {
      string[] parts = CIpAddress.GetParts(ip);
      uint num = 0;
      uint[] numArray = new uint[4]
      {
        16777216U,
        65536U,
        256U,
        1U
      };
      for (int index = 3; index >= 0; --index)
      {
        if (parts[index] != "*")
          num += uint.Parse(parts[index]) * numArray[index];
      }
      return num;
    }

    public static string fromNumber(uint number)
    {
      string[] strArray = new string[4];
      uint[] numArray = new uint[4]
      {
        16777216U,
        65536U,
        256U,
        1U
      };
      for (int index = 0; index < 4; ++index)
      {
        uint num = (number - number % numArray[index]) / numArray[index];
        if (num > (uint) byte.MaxValue)
          throw ExceptionFactory.Create("Wrong binary IP address {0}", (object) number);
        strArray[index] = num.ToString();
        number %= numArray[index];
      }
      return string.Join(".", strArray);
    }

    public static string ApplyMask(string ip, string mask)
    {
      if (!CIpAddress.IsValidTemplated(ip))
        throw ExceptionFactory.Create("Wrong IP address '{0}'", (object) ip);
      if (!CIpAddress.IsValidTemplated(mask))
        throw ExceptionFactory.Create("Wrong mask '{0}'", (object) mask);
      return CIpAddress.fromNumber(CIpAddress.ToNumber(ip) & CIpAddress.ToNumber(mask));
    }

    public static string CreateDefaultMask(int part0)
    {
      if (part0 <= (int) sbyte.MaxValue)
        return "255.0.0.0";
      return part0 > (int) sbyte.MaxValue && part0 <= 191 ? "255.255.0.0" : "255.255.255.0";
    }

    public static bool TryGetPart0(string ipAddress, out int part0)
    {
      part0 = 0;
      if (string.IsNullOrEmpty(ipAddress))
        return false;
      string[] parts = CIpAddress.GetParts(ipAddress);
      return parts.Length == 4 && int.TryParse(parts[0], out part0);
    }

    public static string[] GetParts(string ipAddress)
    {
      return ipAddress.Split('.');
    }

    public static bool IsValidMask(string mask)
    {
      return CIpAddress.IsValidIp(mask) && CIpAddress.IsContinuous(mask);
    }

    public static byte GetMaskLength(string mask)
    {
      byte num = 0;
      foreach (char ch in CIpAddress.ConvertToBin(mask).ToCharArray())
      {
        if (ch == '1')
          ++num;
        if (ch == '0')
          return num;
      }
      return num;
    }

    public static string CalcRangeLastAddress(string ip, string mask)
    {
      if (!CIpAddress.IsValidTemplated(ip))
        throw ExceptionFactory.Create("Wrong IP address {0}", (object) ip);
      if (!CIpAddress.IsValidTemplated(mask))
        throw ExceptionFactory.Create("Wrong mask {0}", (object) mask);
      if (mask.EqualsNoCase("255.255.255.255"))
        return ip;
      return CIpAddress.fromNumber((uint) (((int) CIpAddress.ToNumber(ip) | ~(int) CIpAddress.ToNumber(mask)) - 1));
    }

    public static string CalcRangeFirstAddress(string ip, string mask)
    {
      return CIpAddress.CalcRangeFirstAddress(ip, mask, 1U);
    }

    public static string CalcRangeFirstAddress(string ip, string mask, uint delta)
    {
      if (!CIpAddress.IsValidTemplated(ip))
        throw ExceptionFactory.Create("Wrong IP address {0}", (object) ip);
      if (!CIpAddress.IsValidMask(mask))
        throw ExceptionFactory.Create("Wrong mask {0}", (object) mask);
      if (mask.EqualsNoCase("255.255.255.255"))
        return ip;
      return CIpAddress.fromNumber((CIpAddress.ToNumber(ip) & CIpAddress.ToNumber(mask)) + delta);
    }

    public static string MaskFromNumber(int number)
    {
      return CIpAddress.fromNumber(BitConverterEx.ReverseBits((uint) Math.Pow(2.0, (double) number) - 1U));
    }

    public static bool IsNetContainsIp(string networkIp, int mask, string ip)
    {
      return CIpAddress.IsNetContainsIp(networkIp, CIpAddress.MaskFromNumber(mask), ip);
    }

    public static bool IsNetContainsIp(string networkIp, string mask, string ip)
    {
      string ip1 = CIpAddress.CalcRangeFirstAddress(networkIp, mask);
      string ip2 = CIpAddress.CalcRangeLastAddress(networkIp, mask);
      uint number1 = CIpAddress.ToNumber(ip1);
      uint number2 = CIpAddress.ToNumber(ip2);
      uint number3 = CIpAddress.ToNumber(ip);
      return number1 <= number3 && number3 <= number2;
    }

    private static bool IsContinuous(string mask)
    {
      string bin = CIpAddress.ConvertToBin(mask);
      bool flag = false;
      foreach (char ch in bin.ToCharArray())
      {
        if (flag && ch == '1')
          return false;
        if (!flag && ch == '0')
          flag = true;
      }
      return true;
    }

    private static string ConvertToBin(string ipAddress)
    {
      string[] parts = CIpAddress.GetParts(ipAddress);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string s in parts)
      {
        int result;
        int.TryParse(s, out result);
        string str = Convert.ToString(result, 2).PadLeft(8, '0');
        stringBuilder.AppendFormat("{0}.", (object) str);
      }
      return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
    }

    public static bool IsValidIp(string ipAddress)
    {
      if (string.IsNullOrEmpty(ipAddress))
        return false;
      string[] parts = CIpAddress.GetParts(ipAddress);
      if (parts.Length != 4 || parts[0] == "*" || parts[0] == "0")
        return false;
      foreach (string s in parts)
      {
        int result;
        if (s.Length > 3 || !int.TryParse(s, out result) || (result < 0 || result > (int) byte.MaxValue))
          return false;
      }
      return true;
    }

    public static bool IsValidTemplated(string ipAddress)
    {
      if (string.IsNullOrEmpty(ipAddress))
        return false;
      string[] parts = CIpAddress.GetParts(ipAddress);
      if (parts.Length != 4 || parts[0] == "*" || parts[0] == "0")
        return false;
      foreach (string s in parts)
      {
        int result;
        if (s.Length > 3 || s != "*" && (!int.TryParse(s, out result) || (result < 0 || result > (int) byte.MaxValue)))
          return false;
      }
      return true;
    }

    public static bool IsEmptyIpAddress(string ipString)
    {
      IPAddress ipAddress = (IPAddress) null;
      CIpAddress.ParseIpAddress(ipString, out ipAddress);
      foreach (byte addressByte in ipAddress.GetAddressBytes())
      {
        if (addressByte != (byte) 0)
          return false;
      }
      return true;
    }

    public static bool IsApipaAddress(string ipString)
    {
      IPAddress ipAddress = (IPAddress) null;
      CIpAddress.ParseIpv4Address(ipString, out ipAddress);
      byte[] addressBytes = ipAddress.GetAddressBytes();
      return addressBytes[0] == (byte) 169 && addressBytes[1] == (byte) 254;
    }

    public static bool IsInPrivateSpace(string ip)
    {
      return CIpAddress.ApplyMask(ip, "255.255.0.0") == "192.168.0.0" || CIpAddress.ApplyMask(ip, "255.240.0.0") == "172.16.0.0" || (CIpAddress.ApplyMask(ip, "255.0.0.0") == "10.0.0.0" || CIpAddress.ApplyMask(ip, "255.255.0.0") == "169.254.0.0");
    }

    public static bool IsLoopbackAddress(string ip)
    {
      return CIpAddress.ApplyMask(ip, "255.0.0.0") == "127.0.0.0";
    }

    public static bool IsInPublicSpace(string ip)
    {
      return !CIpAddress.IsLoopbackAddress(ip) && !CIpAddress.IsInPrivateSpace(ip);
    }

    private static void ParseIpv4Address(string ipString, out IPAddress ipAddress)
    {
      CIpAddress.ParseIpAddress(ipString, out ipAddress);
      if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
        throw ExceptionFactory.Create("IP-address is not IPv4", (object) ipString);
    }

    private static void ParseIpAddress(string ipString, out IPAddress ipAddress)
    {
      if (!IPAddress.TryParse(ipString, out ipAddress))
        throw ExceptionFactory.Create("Invalid IP-address", (object) ipString);
    }

    public static int Compare(string ipStr1, string ipStr2)
    {
      IPAddress ipAddress1;
      CIpAddress.ParseIpv4Address(ipStr1, out ipAddress1);
      IPAddress ipAddress2;
      CIpAddress.ParseIpv4Address(ipStr2, out ipAddress2);
      byte[] addressBytes1 = ipAddress1.GetAddressBytes();
      byte[] addressBytes2 = ipAddress2.GetAddressBytes();
      if (addressBytes1.Length != addressBytes2.Length)
        throw ExceptionFactory.Create("Invalid IP addresses.");
      for (int index = 0; index < addressBytes1.Length; ++index)
      {
        if ((int) addressBytes1[index] > (int) addressBytes2[index])
          return 1;
        if ((int) addressBytes1[index] < (int) addressBytes2[index])
          return -1;
      }
      return 0;
    }

    public static string[] GetIpAddressesStr(string hostName)
    {
      if (!CIpAddress.IsValidIp(hostName))
        return ((IEnumerable<IPAddress>) CIpAddress.GetIpAddresses(hostName)).Select<IPAddress, string>((Func<IPAddress, string>) (addr => addr.ToString())).ToArray<string>();
      return new string[1]{ hostName };
    }

    public static string[] GetIpAddressesStr(IPHostEntry ipHostEntry)
    {
      return ((IEnumerable<IPAddress>) CIpAddress.GetIpAddresses(ipHostEntry)).Select<IPAddress, string>((Func<IPAddress, string>) (addr => addr.ToString())).ToArray<string>();
    }

    public static IPAddress[] GetIpAddresses(string hostName)
    {
      if (CIpAddress.IsValidIp(hostName))
      {
        IPAddress address = IPAddress.Parse(hostName);
        if (IPAddress.IsLoopback(address))
          return CIpAddress.GetIpAddresses(Dns.GetHostEntry(address).HostName);
        return new IPAddress[1]{ address };
      }
      IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
      if (hostEntry == null)
        throw ExceptionFactory.Create("Unable to obtain IP addresses for host '{0}'", (object) hostName);
      if (hostName.ToLower() == "localhost" && hostEntry.HostName.ToLower() != "localhost")
        return CIpAddress.GetIpAddresses(hostEntry.HostName);
      return CIpAddress.GetIpAddresses(hostEntry);
    }

    public static IPAddress[] GetIpAddressesSafe(string hostName)
    {
      try
      {
        return CIpAddress.GetIpAddresses(hostName);
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        return new IPAddress[0];
      }
    }

    public static IPAddress[] GetIpAddresses(IPHostEntry ipHostEntry)
    {
      return ((IEnumerable<IPAddress>) ipHostEntry.AddressList).Where<IPAddress>((Func<IPAddress, bool>) (addr => addr.AddressFamily == AddressFamily.InterNetwork)).ToArray<IPAddress>();
    }

    public static int ToInteger(this IPAddress ip)
    {
      byte[] addressBytes = ip.GetAddressBytes();
      return (int) addressBytes[0] << 24 | (int) addressBytes[1] << 16 | (int) addressBytes[2] << 8 | (int) addressBytes[3];
    }

    public static IPAddress FromInteger(int ipAsInt)
    {
      byte[] bytes = BitConverter.GetBytes(ipAsInt);
      Array.Reverse((Array) bytes);
      return new IPAddress(bytes);
    }

    public static int Compare(this IPAddress ip1, IPAddress ip2)
    {
      int integer1 = ip1.ToInteger();
      int integer2 = ip2.ToInteger();
      return integer1 - integer2 >> 31 | (int) ((uint) -(integer1 - integer2) >> 31);
    }
  }
}
