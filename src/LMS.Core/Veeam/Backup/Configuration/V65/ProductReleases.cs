using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal static class ProductReleases
    {
        public static ProductReleaseAttributes Load(string content)
        {
            ProductReleasesParser productReleasesParser = new ProductReleasesParser(new XmlLoader(new string[1]
            {
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema
  attributeFormDefault=""unqualified""
  elementFormDefault=""qualified""
  targetNamespace=""http://tempuri.org/ProductReleases.xsd""
  xmlns=""http://tempuri.org/ProductReleases.xsd""
  xmlns:msc=""http://tempuri.org/ProductReleases.xsd""
  xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

  <xs:element name=""product"" type=""productelement"">
    <xs:unique name=""versionunique"">
      <xs:selector xpath=""msc:release""/>
      <xs:field xpath=""@version""/>
    </xs:unique>
  </xs:element>

  <xs:complexType name=""productelement"">
    <xs:sequence>
      <xs:element name=""release"" type=""releaseelement"" minOccurs=""1"" maxOccurs=""unbounded""/>
    </xs:sequence>
    <xs:attribute name=""upgradable"" type=""versionattribute"" use=""optional"" default=""0.0.0.0""/>
    <xs:attribute name=""preview"" type=""xs:boolean"" use=""optional"" default=""false""/>
  </xs:complexType>

  <xs:complexType name=""releaseelement"">
    <xs:simpleContent>
      <xs:extension base=""xs:string"">
        <xs:attribute name=""version"" type=""versionattribute"" use=""required""/>
        <xs:attribute name=""update"" type=""updateattribute"" use=""optional""/>
        <xs:attribute name=""database"" type=""databaseattribute"" use=""optional""/>
        <xs:attribute name=""supported"" type=""xs:boolean"" use=""required""/>
        <xs:attribute name=""name"" type=""nameattribute"" use=""required""/>
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:simpleType name=""databaseattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""[\d]+(;[\d]+)?""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""versionattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""[\d]+\.[\d]+\.[\d]+\.[\d]+""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""nameattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""updateattribute"">
    <xs:restriction base=""xs:integer"">
      <xs:minInclusive value=""0""/>
    </xs:restriction>
  </xs:simpleType>
  
</xs:schema>"
            }).LoadFromContent(content));
            productReleasesParser.Parse();
            return new ProductReleaseAttributes(productReleasesParser.Upgdarable, productReleasesParser.Preview);
        }

        public static IEnumerable<IProductRelease> Load(
            string content,
            Version productVersion,
            IDatabaseVersion productDatabaseVersion)
        {
            if (productVersion == (Version) null)
                throw new ArgumentNullException(nameof (productVersion));
            if (productDatabaseVersion == null)
                throw new ArgumentNullException(nameof (productDatabaseVersion));
            ProductReleasesParser productReleasesParser = new ProductReleasesParser(new XmlLoader(new string[1]
            {
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema
  attributeFormDefault=""unqualified""
  elementFormDefault=""qualified""
  targetNamespace=""http://tempuri.org/ProductReleases.xsd""
  xmlns=""http://tempuri.org/ProductReleases.xsd""
  xmlns:msc=""http://tempuri.org/ProductReleases.xsd""
  xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

  <xs:element name=""product"" type=""productelement"">
    <xs:unique name=""versionunique"">
      <xs:selector xpath=""msc:release""/>
      <xs:field xpath=""@version""/>
    </xs:unique>
  </xs:element>

  <xs:complexType name=""productelement"">
    <xs:sequence>
      <xs:element name=""release"" type=""releaseelement"" minOccurs=""1"" maxOccurs=""unbounded""/>
    </xs:sequence>
    <xs:attribute name=""upgradable"" type=""versionattribute"" use=""optional"" default=""0.0.0.0""/>
    <xs:attribute name=""preview"" type=""xs:boolean"" use=""optional"" default=""false""/>
  </xs:complexType>

  <xs:complexType name=""releaseelement"">
    <xs:simpleContent>
      <xs:extension base=""xs:string"">
        <xs:attribute name=""version"" type=""versionattribute"" use=""required""/>
        <xs:attribute name=""update"" type=""updateattribute"" use=""optional""/>
        <xs:attribute name=""database"" type=""databaseattribute"" use=""optional""/>
        <xs:attribute name=""supported"" type=""xs:boolean"" use=""required""/>
        <xs:attribute name=""name"" type=""nameattribute"" use=""required""/>
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:simpleType name=""databaseattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""[\d]+(;[\d]+)?""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""versionattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""[\d]+\.[\d]+\.[\d]+\.[\d]+""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""nameattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""updateattribute"">
    <xs:restriction base=""xs:integer"">
      <xs:minInclusive value=""0""/>
    </xs:restriction>
  </xs:simpleType>
  
</xs:schema>"
            }).LoadFromContent(content));
            productReleasesParser.Parse();
            List<IProductRelease> productReleaseList = new List<IProductRelease>();
            foreach (IProductRelease release in productReleasesParser.Releases)
            {
                if (release.IsCurrent)
                    productReleaseList.Add((IProductRelease) new ProductRelease(release.Name, productVersion, release.UpdateNumber, productDatabaseVersion, release.IsSupported, release.IsCurrent));
                else
                    productReleaseList.Add(release);
            }
            return (IEnumerable<IProductRelease>) productReleaseList;
        }
    }
}
