using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LMS.Core.Common;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  [Serializable]
  public class CIPPort : IEquatable<CIPPort>
  {
    private const string IP_TAG = "Ip";
    private const string PORT_TAG = "Port";
    private const string ROOT_TAG = "Root";
    private CIPPort[] _ipsAndHostnames;

    public string Ip { get; private set; }

    public ushort Port { get; private set; }

    public CIPPort(string ip, ushort port)
    {
      this.Ip = ip;
      this.Port = port;
    }

    public string Serialize()
    {
      XmlDocument doc = new XmlDocument();
      XmlNode node = (XmlNode) CXmlHelper.AddElement(doc, "Root");
      CXmlHelper2.AddAttr(node, "Ip", this.Ip);
      CXmlHelper2.AddAttr(node, "Port", this.Port);
      return doc.InnerXml;
    }

    public static CIPPort Deserialize(string data)
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(data);
      XmlNode firstNodeByName = CXmlHelper.GetFirstNodeByName(doc, "Root");
      return new CIPPort(CXmlHelper2.GetAttrAsString(firstNodeByName, "Ip"), CXmlHelper2.GetAttrAsUInt16(firstNodeByName, "Port"));
    }

    public override string ToString()
    {
      return string.Format("[IP:{0}, port:{1}]", (object) this.Ip, (object) this.Port);
    }

    public string ToAgentString()
    {
      return string.Format("{0}:{1}", (object) this.Ip, (object) this.Port);
    }

    public static CIPPort ParseAgentString(string str)
    {
      string[] strArray = str.Split(':');
      if (strArray.Length != 2)
        throw ExceptionFactory.Create("Invalid IP endpoint string.");
      return new CIPPort(strArray[0], ushort.Parse(strArray[1]));
    }

    public override int GetHashCode()
    {
      return this.Ip.ToLower(CultureInfo.InvariantCulture).GetHashCode() ^ this.Port.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      CIPPort ipPort = obj as CIPPort;
      if (ipPort != null)
        return this.Equals(ipPort);
      return false;
    }

    public bool Equals(CIPPort ipPort)
    {
      if (ipPort.Ip.ToLower(CultureInfo.InvariantCulture) == this.Ip.ToLower(CultureInfo.InvariantCulture))
        return (int) ipPort.Port == (int) this.Port;
      return false;
    }

    public static bool Equals(CIPPort x, CIPPort y)
    {
      if (object.ReferenceEquals((object) x, (object) null))
        return object.ReferenceEquals((object) y, (object) null);
      if (object.ReferenceEquals((object) y, (object) null))
        return false;
      return x.Equals(y);
    }

    public IEnumerable<CIPPort> AllIpsAndHostnames
    {
      get
      {
        if (this._ipsAndHostnames == null)
          this._ipsAndHostnames = this.GetAllIpsAndHostnames().ToArray<CIPPort>();
        return (IEnumerable<CIPPort>) this._ipsAndHostnames.Copy<CIPPort>();
      }
    }

    private IEnumerable<CIPPort> GetAllIpsAndHostnames()
    {
      try
      {
        return ((IEnumerable<IPAddress>) CIpAddress.GetIpAddresses(this.Ip)).Select<IPAddress, CIPPort>((Func<IPAddress, CIPPort>) (addr => new CIPPort(addr.ToString(), this.Port))).Union<CIPPort>(new CIPPort[1]
        {
          this
        });
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        return (IEnumerable<CIPPort>) new CIPPort[1]
        {
          this
        };
      }
    }

    public static bool AnyIntersectionsByIpAndDns(
      IEnumerable<CIPPort> firstSequence,
      IEnumerable<CIPPort> secondSequence)
    {
      firstSequence = (IEnumerable<CIPPort>) firstSequence.ToArray<CIPPort>();
      secondSequence = (IEnumerable<CIPPort>) secondSequence.ToArray<CIPPort>();
      Log.Debug("Looking for intersections between {0} and {1}", (object) string.Join<CIPPort>(",", firstSequence), (object) string.Join<CIPPort>(",", secondSequence));
      IEnumerable<CIPPort> array1 = (IEnumerable<CIPPort>) firstSequence.SelectMany<CIPPort, CIPPort>((Func<CIPPort, IEnumerable<CIPPort>>) (item => item.AllIpsAndHostnames)).ToArray<CIPPort>();
      Log.Debug("{0} is resolved to {1}", (object) string.Join<CIPPort>(",", firstSequence), (object) string.Join<CIPPort>(",", array1));
      IEnumerable<CIPPort> array2 = (IEnumerable<CIPPort>) secondSequence.SelectMany<CIPPort, CIPPort>((Func<CIPPort, IEnumerable<CIPPort>>) (item => item.AllIpsAndHostnames)).ToArray<CIPPort>();
      Log.Debug("{0} is resolved to {1}", (object) string.Join<CIPPort>(",", secondSequence), (object) string.Join<CIPPort>(",", array2));
      bool flag = array1.Intersect<CIPPort>(array2).Any<CIPPort>();
      Log.Debug("Intersection found: {0}", (object) flag);
      return flag;
    }

    public static IPEndPoint ToIpEndPoint(CIPPort ipPort)
    {
      if (CIpAddress.IsValidIp(ipPort.Ip))
        return new IPEndPoint(IPAddress.Parse(ipPort.Ip), (int) ipPort.Port);
      return new IPEndPoint(((IEnumerable<IPAddress>) CIpAddress.GetIpAddresses(ipPort.Ip)).First<IPAddress>(), (int) ipPort.Port);
    }

    public static CIPPort ConvertHostnameToIPAddress(CIPPort ipPort)
    {
      if (CIpAddress.IsValidIp(ipPort.Ip))
        return ipPort;
      return new CIPPort(((IEnumerable<IPAddress>) CIpAddress.GetIpAddresses(ipPort.Ip)).First<IPAddress>().ToString(), ipPort.Port);
    }
  }
}
