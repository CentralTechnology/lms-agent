using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LMS.Core.Veeam.Backup.Common
{
  public class ReadableTable : IDisposable
  {
    protected readonly CDisposableList Disposables = new CDisposableList();
    [CanBeNull]
    protected DataTable CurrentTable;

    protected internal ReadableTable()
    {
    }

    protected internal ReadableTable(DataTable table)
    {
      Exceptions.CheckArgumentNullException<DataTable>(table, nameof (table));
      this.CurrentTable = table;
    }

    public ReadableTable(
      IDatabaseAccessor accessor,
      DbQueryOptions options,
      string spName,
      params SqlParameter[] spParams)
    {
      this.CurrentTable = accessor.GetDataTable(spName, options.IsRetryable, spParams);
      this.Disposables.Add<DataTable>(this.CurrentTable);
    }

    public void Dispose()
    {
      this.Disposables.Dispose();
    }

    public T ReadSingleOrDefault<T>(Func<DataTableReader, T> parser)
    {
      return this.ReadSingleOrDefault<T>(parser, default (T));
    }

    public T ReadSingleOrDefault<T>(Func<DataTableReader, T> parser, T defaultValue)
    {
      return this.ReadSingleOrDefaultInternal<T>((ReadableTable.IParserExecutionPolicy<T>) new ReadableTable.ParserWithoutDefaultValueExecutionPolicy<T>(parser), defaultValue);
    }

    public T ReadSingleOrDefault<T>(Func<DataTableReader, T, T> parser)
    {
      return this.ReadSingleOrDefault<T>(parser, default (T));
    }

    public T ReadSingleOrDefault<T>(Func<DataTableReader, T, T> parser, T defaultValue)
    {
      return this.ReadSingleOrDefaultInternal<T>((ReadableTable.IParserExecutionPolicy<T>) new ReadableTable.ParserWithDefaultValueExecutionPolicy<T>(parser, defaultValue), defaultValue);
    }

    private T ReadSingleOrDefaultInternal<T>(
      ReadableTable.IParserExecutionPolicy<T> parserExecutionPolicy,
      T defaultValue)
    {
      if (this.CurrentTable == null || this.CurrentTable.Rows.Count == 0)
        return defaultValue;
      if (this.CurrentTable.Rows.Count > 1)
        throw new InvalidOperationException("Sequence contains more than one element.");
      using (DataTableReader dataReader = this.CurrentTable.CreateDataReader())
      {
        if (!dataReader.Read())
          throw new InvalidOperationException("Cannot read.");
        return parserExecutionPolicy.Execute(dataReader);
      }
    }

    public T ReadSingle<T>(Func<DataTableReader, T> parser)
    {
      if (this.CurrentTable == null)
        throw new Exception("CurrentTable is null");
      if (this.CurrentTable.Rows.Count == 0)
        return default (T);
      if (this.CurrentTable.Rows.Count > 1)
        throw new InvalidOperationException("Sequence contains more than one element.");
      using (DataTableReader dataReader = this.CurrentTable.CreateDataReader())
      {
        if (!dataReader.Read())
          throw new InvalidOperationException("Cannot read.");
        return parser(dataReader);
      }
    }

    public bool IsAnyDataPresent()
    {
      if (this.CurrentTable == null)
        return false;
      return this.CurrentTable.Rows.Count > 0;
    }

    public T[] ReadAll<T>(Func<DataTableReader, T> parser)
    {
      if (this.CurrentTable == null)
        return new T[0];
      T[] objArray = new T[this.CurrentTable.Rows.Count];
      if (objArray.Length == 0)
        return objArray;
      int num = 0;
      using (DataTableReader dataReader = this.CurrentTable.CreateDataReader())
      {
        while (dataReader.Read())
          objArray[num++] = parser(dataReader);
      }
      return objArray;
    }

    public IEnumerable<T> EnumerateAll<T>(Func<DataTableReader, T> parser)
    {
      if (this.CurrentTable != null && this.CurrentTable.Rows.Count != 0)
      {
        using (DataTableReader reader = this.CurrentTable.CreateDataReader())
        {
          while (reader.Read())
            yield return parser(reader);
        }
      }
    }

    private interface IParserExecutionPolicy<T>
    {
      T Execute(DataTableReader reader);
    }

    private class ParserWithoutDefaultValueExecutionPolicy<T> : ReadableTable.IParserExecutionPolicy<T>
    {
      private Func<DataTableReader, T> _parser;

      public ParserWithoutDefaultValueExecutionPolicy(Func<DataTableReader, T> parser)
      {
        this._parser = parser;
      }

      public T Execute(DataTableReader reader)
      {
        return this._parser(reader);
      }
    }

    private class ParserWithDefaultValueExecutionPolicy<T> : ReadableTable.IParserExecutionPolicy<T>
    {
      private Func<DataTableReader, T, T> _parser;
      private T _defaultValue;

      public ParserWithDefaultValueExecutionPolicy(
        Func<DataTableReader, T, T> parser,
        T defaultValue)
      {
        this._parser = parser;
        this._defaultValue = defaultValue;
      }

      public T Execute(DataTableReader reader)
      {
        return this._parser(reader, this._defaultValue);
      }
    }
  }
}
