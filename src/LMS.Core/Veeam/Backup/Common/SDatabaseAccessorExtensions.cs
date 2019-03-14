using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class SDatabaseAccessorExtensions
  {




    public static DataTable GetDataTable(
      this IDatabaseAccessor accessor,
      CStoredProcedureData storedProcedureData)
    {
      return accessor.GetDataTable(storedProcedureData.ProcedureName, storedProcedureData.Parameters.ToArray<SqlParameter>());
    }

    public static ReadableTable GetReadableTable(
      this IDatabaseAccessor accessor,
      string spName,
      params SqlParameter[] spParams)
    {
      DbQueryOptions options = new DbQueryOptions();
      return accessor.GetReadableTable(options, spName, spParams);
    }

    public static ReadableTable GetReadableTable(
      this IDatabaseAccessor accessor,
      CStoredProcedureData storedProcedureData)
    {
      return accessor.GetReadableTable(storedProcedureData.ProcedureName, storedProcedureData.Parameters.ToArray<SqlParameter>());
    }


    public static ReadableTable GetReadableTable(
      this IDatabaseAccessor accessor,
      DbQueryOptions options,
      string spName,
      params SqlParameter[] spParams)
    {
      return new ReadableTable(accessor, options, spName, spParams);
    }


    public static T GetSingleOrDefault<T>(
      this IDatabaseAccessor accessor,
      string spName,
      Func<DataTableReader, T> parser,
      params SqlParameter[] spParams)
    {
      using (ReadableTable readableTable = accessor.GetReadableTable(spName, spParams))
        return readableTable.ReadSingleOrDefault<T>(parser);
    }

    public static T GetSingle<T>(
      this IDatabaseAccessor accessor,
      string spName,
      Func<DataTableReader, T> parser,
      params SqlParameter[] spParams)
    {
      using (ReadableTable readableTable = accessor.GetReadableTable(spName, spParams))
        return readableTable.ReadSingle<T>(parser);
    }

    public static T[] GetAll<T>(
      this IDatabaseAccessor accessor,
      string spName,
      Func<DataTableReader, T> parser,
      params SqlParameter[] spParams)
    {
      using (ReadableTable readableTable = accessor.GetReadableTable(spName, spParams))
        return readableTable.ReadAll<T>(parser);
    }
  }
}
