using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductServers
    {
        public IList<IProductServer> Servers { get; private set; }

        public Version MinSupportedVersion { get; private set; }

        private ProductServers()
        {
        }

        public static ProductServers Load(string content)
        {
            ProductServersParser productServersParser = new ProductServersParser(new XmlLoader(new string[1]
            {
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema
  attributeFormDefault=""unqualified""
  elementFormDefault=""qualified""
  targetNamespace=""http://tempuri.org/ProductServers.xsd""
  xmlns=""http://tempuri.org/ProductServers.xsd""
  xmlns:msc=""http://tempuri.org/ProductServers.xsd""
  xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

  <xs:element name=""product"" type=""productelement"">
    <xs:unique name=""versionunique"">
      <xs:selector xpath=""msc:server""/>
      <xs:field xpath=""@version""/>
    </xs:unique>
    <xs:unique name=""versionfamilyunique"">
      <xs:selector xpath=""msc:server""/>
      <xs:field xpath=""@family""/>
      <xs:field xpath=""@version""/>
    </xs:unique>
    <xs:keyref name=""dummy"" refer=""versionunique"">
      <xs:selector xpath="".""/>
      <xs:field xpath=""@minsupportedversion""/>
    </xs:keyref>
  </xs:element>

  <xs:complexType name=""productelement"">
    <xs:sequence>
      <xs:element name=""server"" type=""serverelement"" minOccurs=""1"" maxOccurs=""unbounded""/>
    </xs:sequence>
    <xs:attribute name=""minsupportedversion"" type=""versionattribute"" use=""optional""/>
  </xs:complexType>

  <xs:complexType name=""serverelement"">
    <xs:simpleContent>
      <xs:extension base=""xs:string"">
        <xs:attribute name=""family"" type=""familyattribute"" use=""required""/>
        <xs:attribute name=""version"" type=""versionattribute"" use=""required""/>
        <xs:attribute name=""servicepack"" type=""xs:integer"" default=""0"" use=""optional""/>
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:simpleType name=""familyattribute"">
    <xs:restriction base=""xs:string"">
      <xs:enumeration value=""sql2000""/>
      <xs:enumeration value=""sql2005""/>
      <xs:enumeration value=""sql2008""/>
      <xs:enumeration value=""sql2008r2""/>
      <xs:enumeration value=""sql2012""/>
      <xs:enumeration value=""sql2014""/>
      <xs:enumeration value=""sql2016""/>
      <xs:enumeration value=""sql2017""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""versionattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""[\d]+\.[\d]+\.[\d]+\.[\d]+""/>
    </xs:restriction>
  </xs:simpleType>
  
</xs:schema>"
            }).LoadFromContent(content));
            productServersParser.Parse();
            List<IProductServer> list = productServersParser.Servers.ToList<IProductServer>();
            Version version1 = productServersParser.MinSupportedVersion;
            if ((object) version1 == null)
                version1 = list.Min<IProductServer, Version>((Func<IProductServer, Version>) (s => s.Version));
            Version version2 = version1;
            return new ProductServers()
            {
                Servers = (IList<IProductServer>) list,
                MinSupportedVersion = version2
            };
        }
    }
}
