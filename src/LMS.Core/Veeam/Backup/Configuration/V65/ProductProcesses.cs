using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class ProductProcesses : IProductProcesses, IEnumerable<IProductProcess>, IEnumerable
  {
    private readonly IList<IProductProcess> _processes;

    private ProductProcesses()
    {
      this._processes = (IList<IProductProcess>) new List<IProductProcess>();
    }

    private ProductProcesses(IEnumerable<IProductProcess> processes)
    {
      if (processes == null)
        throw new ArgumentNullException(nameof (processes));
      this._processes = (IList<IProductProcess>) new List<IProductProcess>(processes);
    }

    IProductProcess IProductProcesses.this[Guid id]
    {
      get
      {
        return this._processes.First<IProductProcess>((Func<IProductProcess, bool>) (process => process.Id == id));
      }
    }

    IEnumerable<IProductProcessNode> IProductProcesses.Items
    {
      get
      {
        return (IEnumerable<IProductProcessNode>) this._processes.Where<IProductProcess>((Func<IProductProcess, bool>) (process => !((IEnumerable<ProductProcessOwner>) process.Owners).Any<ProductProcessOwner>())).Select<IProductProcess, ProductProcessNode>((Func<IProductProcess, ProductProcessNode>) (process => new ProductProcessNode((IEnumerable<IProductProcess>) this._processes, (IProductProcessNode) null, process)));
      }
    }

    IEnumerator<IProductProcess> IEnumerable<IProductProcess>.GetEnumerator()
    {
      return this._processes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._processes.GetEnumerator();
    }

    public static IProductProcesses Empty
    {
      get
      {
        return (IProductProcesses) new ProductProcesses();
      }
    }

    public static IProductProcesses Load(string content)
    {
      return (IProductProcesses) new ProductProcesses(new ProductProcessesParser(new XmlLoader(new string[1]
      {
        @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema
  attributeFormDefault=""unqualified""
  elementFormDefault=""qualified""
  targetNamespace=""http://tempuri.org/ProductProcesses.xsd""
  xmlns=""http://tempuri.org/ProductProcesses.xsd""
  xmlns:msc=""http://tempuri.org/ProductProcesses.xsd""
  xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

  <xs:element name=""product"" type=""productelement"">
    <xs:unique name=""executableunique"">
      <xs:selector xpath=""msc:process|msc:service""/>
      <xs:field xpath=""@id""/>
      <xs:field xpath=""@platform""/>
    </xs:unique>
 <!--
    <xs:unique name=""installunique"">
      <xs:selector xpath="".//msc:install"" />
      <xs:field xpath=""@productcode""/>
      <xs:field xpath=""@componentid""/>
    </xs:unique>
-->
  </xs:element>

  <xs:complexType name=""productelement"">
    <xs:sequence>
      <xs:choice minOccurs=""1"" maxOccurs=""unbounded"">
        <xs:element name=""process"" type=""processelement"">
          <xs:unique name=""processownerunique"">
            <xs:selector xpath=""msc:owner""/>
            <xs:field xpath=""@id""/>
            <xs:field xpath=""@platform""/>
          </xs:unique>
        </xs:element>
        <xs:element name=""service"" type=""serviceelement""/>
      </xs:choice>
    </xs:sequence>
  </xs:complexType>

  <xs:complexType name=""executableelement"">
    <xs:sequence>
      <xs:element name=""install"" type=""installelement"" minOccurs=""0"" maxOccurs=""1""/>
    </xs:sequence>
    <xs:attribute name=""id"" type=""guidattribute"" use=""required""/>
    <xs:attribute name=""platform"" type=""platformattribute"" use=""required""/>
    <xs:attribute name=""type"" type=""typeattribute"" default=""none"" use=""optional""/>
    <xs:attribute name=""name"" type=""nameattribute"" use=""required""/>
    <xs:attribute name=""filename"" type=""filenameattribute"" use=""required""/>
    <xs:attribute name=""description"" type=""descriptionattribute"" use=""optional""/>
  </xs:complexType>

  <xs:complexType name=""processelement"">
    <xs:complexContent>
      <xs:extension base=""executableelement"">
        <xs:sequence>
          <xs:element name=""owner"" type=""ownerelement"" minOccurs=""0"" maxOccurs=""unbounded""/>
        </xs:sequence>
    </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name=""serviceelement"">
    <xs:complexContent>
      <xs:extension base=""executableelement"">
        <xs:attribute name=""order"" type=""xs:integer"" default=""0"" use=""optional""/>
        <xs:attribute name=""servicename"" type=""servicenameattribute"" use=""required""/>
        <xs:attribute name=""canbedisabled"" type=""xs:boolean"" default=""false"" use=""optional""/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name=""ownerelement"">
    <xs:simpleContent>
      <xs:extension base=""xs:string"">
        <xs:attribute name=""id"" type=""guidattribute"" use=""required""/>
        <xs:attribute name=""platform"" type=""platformattribute"" use=""required""/>
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:complexType name=""installelement"">
    <xs:simpleContent>
      <xs:extension base=""xs:string"">
        <xs:attribute name=""productcode"" type=""guidattribute"" use=""required""/>
        <xs:attribute name=""componentid"" type=""guidattribute"" use=""required""/>
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:simpleType name=""guidattribute"">
    <xs:restriction base=""xs:string"">
      <xs:pattern value=""{([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}}""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""platformattribute"">
    <xs:restriction base=""xs:string"">
      <xs:enumeration value=""win32""/>
      <xs:enumeration value=""win64""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""typeattribute"">
    <xs:restriction base=""xs:string"">
      <xs:enumeration value=""none""/>
      <xs:enumeration value=""gui""/>
      <xs:enumeration value=""cui""/>
      <xs:enumeration value=""service""/>
      <xs:enumeration value=""component""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""nameattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""filenameattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""servicenameattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>

  <xs:simpleType name=""descriptionattribute"">
    <xs:restriction base=""xs:string"">
      <xs:minLength value=""1""/>
    </xs:restriction>
  </xs:simpleType>
  
</xs:schema>"
      }).LoadFromContent(content)).Parse());
    }
  }
}
