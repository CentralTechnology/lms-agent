using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Configuration.V65;
using LMS.Core.Veeam.DBManager;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CDbAccessor
  {
    public static SqlParameter MakeParam(string name, bool value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.Bit);
    }

    public static SqlParameter MakeParam(string name, DateTime value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.DateTime);
    }

    public static SqlParameter MakeParam(string name, short value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.SmallInt);
    }

    public static SqlParameter MakeParam(string name, ushort value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.SmallInt);
    }

    public static SqlParameter MakeParam(string name, int value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.Int);
    }

    public static SqlParameter MakeParam(string name, uint value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.Int);
    }

    public static SqlParameter MakeParam(string name, long value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.BigInt);
    }

    public static SqlParameter MakeParam(string name, ulong value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.BigInt);
    }

    public static SqlParameter MakeParam(string name, string value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.NVarChar);
    }

    public static SqlParameter MakeParam(string name, Guid value)
    {
      return CDbAccessor.MakeParam(name, (object) value, SqlDbType.UniqueIdentifier);
    }

    public static SqlParameter MakeParam(string name, ISqlTableType value)
    {
      return CDbAccessor.MakeParam(name, (object) value.Table, SqlDbType.Structured);
    }

    public static SqlParameter MakeParam(string name, DBNull value)
    {
      SqlParameter sqlParameter = new SqlParameter();
      sqlParameter.ParameterName = name;
      sqlParameter.Value = (object) value;
      return sqlParameter;
    }

    public static SqlParameter MakeParam(string name, object value)
    {
      SqlParameter sqlParameter = new SqlParameter();
      sqlParameter.ParameterName = name;
      sqlParameter.Value = value ?? (object) DBNull.Value;
      return sqlParameter;
    }

    public static SqlParameter MakeParam(string name, object value, SqlDbType type)
    {
      SqlParameter sqlParameter = new SqlParameter(name, type);
      sqlParameter.Value = value ?? (object) DBNull.Value;
      return sqlParameter;
    }

    public static string SqlParamsToString(IEnumerable<SqlParameter> spParams)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (SqlParameter spParam in spParams)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(", ");
        stringBuilder.AppendFormat("{0} = {1}", (object) spParam.ParameterName, spParam.Value);
      }
      return stringBuilder.ToString();
    }

    public static IDatabaseAccessor CreateAssessor(
      string connectionString,
      IDatabaseConfiguration configuration)
    {
      return (IDatabaseAccessor) new LocalDbAccessor(connectionString, configuration);
    }

    public static string BuildListXml(IEnumerable<IHasXmlNode> ids)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<ROOT>");
      if (ids != null)
      {
        foreach (IHasXmlNode id in ids)
          stringBuilder.Append(id.GetNode().OuterXml);
      }
      stringBuilder.Append("</ROOT>");
      return stringBuilder.ToString();
    }

    public static string BuildListXml<T>(IEnumerable<T> ids, string name)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<ROOT>");
      if (ids != null)
      {
        foreach (T id in ids)
          stringBuilder.Append(CDbAccessor.GetNode(name, name, (object) id));
      }
      stringBuilder.Append("</ROOT>");
      return stringBuilder.ToString();
    }

    private static string GetNode(string nodeName, string attributeName, object value)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlAttribute attribute = xmlDocument.CreateAttribute(attributeName);
      attribute.InnerText = value.ToString();
      XmlElement element = xmlDocument.CreateElement(nodeName);
      element.Attributes.Append(attribute);
      return element.OuterXml;
    }
  }
}
