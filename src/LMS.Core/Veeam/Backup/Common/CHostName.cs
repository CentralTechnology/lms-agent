using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public class CHostName
  {
    public static readonly string INVALID_NET_BIOS_NAME_ERROR = string.Format("Windows computer name cannot be more than 15 characters long, be entirely numeric, or contain the following characters: {0}", (object) " `~!@#$%^&*()=+_[]{}\\|;:.'\",<>/?.");
    private const string InvalidCharsRegexp = "[^a-zA-Z0-9\\-]+";
    private const string InvalidStartChars = "^[0-9\\-]+";
    private const string InvalidEndChars = "\\-+$";
    private const string INVALID_NET_BIOS_NAME_CHARS = " `~!@#$%^&*()=+_[]{}\\|;:.'\",<>/?.";

    public static string FindNetbios(string serverName)
    {
      int length = serverName.IndexOf('.');
      if (-1 == length)
        return serverName;
      IPAddress address;
      if (IPAddress.TryParse(serverName, out address))
        return (string) null;
      return serverName.Substring(0, length);
    }

    public static bool IsFqdnName(string serverName)
    {
      IPAddress address;
      return -1 != serverName.IndexOf('.') && !IPAddress.TryParse(serverName, out address);
    }

    public static bool IsValid(string hostName)
    {
      return !string.IsNullOrEmpty(hostName) && !Regex.IsMatch(hostName, "[^a-zA-Z0-9\\-]+") && (!Regex.IsMatch(hostName, "^[0-9\\-]+") && !Regex.IsMatch(hostName, "\\-+$"));
    }

    public static string MakeValid(string hostName, string defaultHostName)
    {
      string str = CHostName.MakeValidNotTruncated(hostName, defaultHostName);
      if (str.Length > 60)
        str = str.Substring(0, 60);
      return str;
    }

    private static string MakeValidNotTruncated(string hostName, string defaultHostName)
    {
      if (CHostName.IsValid(hostName))
        return hostName;
      if (string.IsNullOrEmpty(hostName))
        return defaultHostName;
      string input = Regex.Replace(Regex.Replace(hostName, "[^a-zA-Z0-9\\-]+", "-"), "\\-+$", "");
      string str = Regex.Replace(input, "^[0-9\\-]+", "");
      if (string.IsNullOrEmpty(str))
        return CHostName.MakeValidNotTruncated(defaultHostName, "n") + input;
      return str;
    }

    public static string TryGetLocalName(bool returnLocalhostIfError = false)
    {
      try
      {
        return Dns.GetHostName();
      }
      catch (SocketException)
      {
        return returnLocalhostIfError ? "localhost" : "UnknownHost";
      }
      catch (Exception)
      {
        return returnLocalhostIfError ? "localhost" : "UnknownHost";
      }
    }

    public static string GetLocalServerFQDN()
    {
      return Dns.GetHostEntry("LocalHost").HostName;
    }

    public static CIPPort GetIpPort(HttpWebRequest webRequest, ushort defaultPort)
    {
      string host = webRequest.Host;
      int length = host.IndexOf(":", StringComparison.InvariantCulture);
      if (length != -1)
        return new CIPPort(host.Substring(0, length), ushort.Parse(host.Substring(length + 1)));
      return new CIPPort(host, defaultPort);
    }

    public static bool IsValidNetBiosName(string netBiosName)
    {
      return !string.IsNullOrEmpty(netBiosName) && netBiosName.Length <= 15 && StringHelper.IsStrValid(netBiosName, " `~!@#$%^&*()=+_[]{}\\|;:.'\",<>/?.");
    }
  }
}
