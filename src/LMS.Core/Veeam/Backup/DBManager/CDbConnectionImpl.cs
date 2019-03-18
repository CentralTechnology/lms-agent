using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Configuration.V65;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CDbConnectionImpl : IDisposable
  {
    private readonly string _connectionString;
    private TimeSpan _timeoutExecSql;

    public CDbConnectionImpl(IDatabaseConfiguration dbConnectionParams, TimeSpan timeoutExecSql)
      : this(dbConnectionParams.ConnectionString, timeoutExecSql)
    {
    }

    public CDbConnectionImpl(string connectionString, TimeSpan timeoutExecSql)
    {
      this._connectionString = connectionString;
      this._timeoutExecSql = timeoutExecSql;
    }

    public DataTable ExecuteQuery(string query)
    {
      return this.ExecuteQuery(query, (SqlParameter[]) null);
    }

    public IDataReader ExecDataReader(
      string spName,
      TimeSpan? timeout,
      params SqlParameter[] spParams)
    {
      try
      {
        using (CAutoRefScope scope = new CAutoRefScope())
        {
          CAutoRef<SqlConnection> connectionRef = new CAutoRef<SqlConnection>(scope, new SqlConnection(this._connectionString));
          connectionRef.Get().Open();
          CAutoRef<SqlCommand> commandRef = new CAutoRef<SqlCommand>(scope, new SqlCommand(spName, connectionRef.Get()));
          SqlCommand sqlCommand = commandRef.Get();
          sqlCommand.Parameters.AddRange(spParams);
          sqlCommand.CommandType = CommandType.StoredProcedure;
          sqlCommand.CommandTimeout = timeout.HasValue ? (int) timeout.Value.TotalSeconds : (int) this._timeoutExecSql.TotalSeconds;
          return (IDataReader) scope.Commit<CLazyDataReader>(new CLazyDataReader(connectionRef, commandRef));
        }
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
    {
      try
      {
        using (SqlConnection connection = new SqlConnection(this._connectionString))
        {
          connection.Open();
          using (SqlCommand selectCommand = new SqlCommand(query, connection))
          {
            selectCommand.CommandTimeout = (int) this._timeoutExecSql.TotalSeconds;
            if (parameters != null)
              selectCommand.Parameters.AddRange(parameters);
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              DataTable dataTable = new DataTable();
              sqlDataAdapter.Fill(dataTable);
              return dataTable;
            }
          }
        }
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    public int ExecuteNonQuery(
      string query,
      CommandType commandType,
      params SqlParameter[] parameters)
    {
      try
      {
        using (SqlConnection connection = new SqlConnection(this._connectionString))
        {
          connection.Open();
          using (SqlCommand sqlCommand = new SqlCommand(query, connection))
          {
            sqlCommand.CommandTimeout = (int) this._timeoutExecSql.TotalSeconds;
            sqlCommand.CommandType = commandType;
            if (parameters != null)
              sqlCommand.Parameters.AddRange(parameters);
            return sqlCommand.ExecuteNonQuery();
          }
        }
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    public DataTable ExecuteStoredProc(
      string procedureName,
      params SqlParameter[] arguments)
    {
      try
      {
        DataTable trgTable = new DataTable();
        this.ExecuteStoredProc(trgTable, procedureName, arguments);
        return trgTable;
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    public void ExecuteStoredProc(
      DataTable trgTable,
      string procedureName,
      params SqlParameter[] arguments)
    {
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(this._connectionString))
        {
          sqlConnection.Open();
          using (SqlCommand selectCommand = new SqlCommand())
          {
            selectCommand.Connection = sqlConnection;
            selectCommand.CommandTimeout = (int) this._timeoutExecSql.TotalSeconds;
            selectCommand.CommandText = procedureName;
            selectCommand.CommandType = CommandType.StoredProcedure;
            selectCommand.Parameters.AddRange(arguments);
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
              sqlDataAdapter.Fill(trgTable);
          }
        }
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    public void ExecBulkCopyOperation(IDataReader sqlBulkReader, string destinationTable)
    {
      try
      {
        using (SqlConnection connection = new SqlConnection(this._connectionString))
        {
          connection.Open();
          using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
          {
            sqlBulkCopy.DestinationTableName = destinationTable;
            sqlBulkCopy.BulkCopyTimeout = 0;
            sqlBulkCopy.WriteToServer(sqlBulkReader);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        this.SafeReconnect();
        throw;
      }
    }

    public void ExecuteNonQueryStoredProcWithTimeout(
      string procedureName,
      TimeSpan timeout,
      params SqlParameter[] arguments)
    {
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(this._connectionString))
        {
          sqlConnection.Open();
          using (SqlCommand sqlCommand = new SqlCommand())
          {
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandTimeout = (int) timeout.TotalSeconds;
            sqlCommand.CommandText = procedureName;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddRange(arguments);
            sqlCommand.ExecuteNonQuery();
          }
        }
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    public void ExecuteNonQueryStoredProc(string procedureName, params SqlParameter[] arguments)
    {
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(this._connectionString))
        {
          sqlConnection.Open();
          using (SqlCommand sqlCommand = new SqlCommand())
          {
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandTimeout = (int) this._timeoutExecSql.TotalSeconds;
            sqlCommand.CommandText = procedureName;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddRange(arguments);
            sqlCommand.ExecuteNonQuery();
          }
        }
      }
      catch (Exception)
      {
        this.SafeReconnect();
        throw;
      }
    }

    private void SafeReconnect()
    {
    }

    void IDisposable.Dispose()
    {
    }
  }
}
