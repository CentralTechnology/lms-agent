using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal static class ProductSetups
    {
        public static IEnumerable<IProductSetup> Load(
            string content,
            Version productVersion,
            Guid packageCode)
        {
            if (productVersion == (Version) null)
                throw new ArgumentNullException(nameof (productVersion));
            ProductSetupsParser productSetupsParser = new ProductSetupsParser(new XmlLoader(new string[1]
            {
                ProductSetupsSchema
            }).LoadFromContent(content));
            List<IProductSetup> productSetupList = new List<IProductSetup>();
            foreach (IProductSetup productSetup in productSetupsParser.Parse())
            {
                if (productSetup.IsCurrent)
                    productSetupList.Add((IProductSetup) new ProductSetup(productVersion, productSetup.Platform, productSetup.FileName, packageCode, productSetup.ProductCode, productSetup.UpgradeCode, productSetup.IsCurrent));
                else
                    productSetupList.Add(productSetup);
            }
            return (IEnumerable<IProductSetup>) productSetupList;
        }

        private const string ProductSetupsSchema = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema
  attributeFormDefault=""unqualified""
  elementFormDefault=""qualified""
  targetNamespace=""http://tempuri.org/ProductSetups.xsd""
  xmlns=""http://tempuri.org/ProductSetups.xsd""
  xmlns:msc=""http://tempuri.org/ProductSetups.xsd""
  xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

  <xs:element name=""product"" type=""productelement"">
    <xs:unique name=""versionunique"">
      <xs:selector xpath=""msc:setup""/>
      <xs:field xpath=""@version""/>
      <xs:field xpath=""@platform""/>
    </xs:unique>
  </xs:element>

  <xs:complexType name=""productelement"">
    <xs:sequence>
      <xs:element name=""setup"" type=""setupelement"" minOccurs=""1"" maxOccurs=""unbounded""/>
    </xs:sequence>
  </xs:complexType>

  <xs:complexType name=""setupelement"">
    <xs:simpleContent>
      <xs:extension base=""xs:string"">
        <xs:attribute name=""version"" type=""versionattribute"" use=""required""/>
        <xs:attribute name=""platform"" type=""platformattribute"" use=""required""/>
        <xs:attribute name=""filename"" type=""filenameattribute"" use=""required""/>
        <xs:attribute name=""packagecode"" type=""guidattribute"" use=""required""/>
        <xs:attribute name=""productcode"" type=""guidattribute"" use=""required""/>
        <xs:attribute name=""upgradecode"" type=""guidattribute"" use=""required""/>
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:simpleType name=""versionattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""[\d]+\.[\d]+\.[\d]+\.[\d]+""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""platformattribute"">
    <xs:restriction base=""xs:string"">
      <xs:enumeration value=""win32""/>
      <xs:enumeration value=""win64""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""filenameattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""guidattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""{([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}}""/>
    </xs:restriction>
  </xs:simpleType>

</xs:schema>";
    }
}
