using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Configuration.V65;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CPersistentDbConnection : IDisposable
  {
    private CDbConnectionImpl m_dbConnection;

    public CPersistentDbConnection(CDbConnectionImpl dbConnection)
    {
      this.m_dbConnection = dbConnection;
    }

    public CPersistentDbConnection(
      IDatabaseConfiguration dbConnectionParams,
      TimeSpan timeoutExecSql)
    {
      this.m_dbConnection = new CDbConnectionImpl(dbConnectionParams, timeoutExecSql);
    }

    public DataTable ExecuteQuery(string query)
    {
      return this.m_dbConnection.ExecuteQuery(query);
    }

    public int ExecuteNonQuery(
      string query,
      CommandType commandType,
      params SqlParameter[] spParams)
    {
      return this.m_dbConnection.ExecuteNonQuery(query, commandType, spParams);
    }

    public IDataReader ExecDataReaderWithTimeout(
      string spName,
      TimeSpan? timeout,
      params SqlParameter[] spParams)
    {
      return this.m_dbConnection.ExecDataReader(spName, timeout, spParams);
    }

    public IDataReader ExecDataReader(string spName, params SqlParameter[] spParams)
    {
      return this.m_dbConnection.ExecDataReader(spName, new TimeSpan?(), spParams);
    }

    public void ExecBulkCopyOperation(IDataReader sqlBulkReader, string destinationTable)
    {
      this.m_dbConnection.ExecBulkCopyOperation(sqlBulkReader, destinationTable);
    }

    public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
    {
      try
      {
        return this.m_dbConnection.ExecuteQuery(query, parameters);
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        throw new Exception("SQL server is not available.", ex);
      }
    }

    public DataTable ExecuteStoredProc(
      string procedureName,
      params SqlParameter[] arguments)
    {
      return this.m_dbConnection.ExecuteStoredProc(procedureName, arguments);
    }

    public void ExecuteStoredProc(
      DataTable trgTable,
      string procedureName,
      params SqlParameter[] arguments)
    {
      try
      {
        this.m_dbConnection.ExecuteStoredProc(trgTable, procedureName, arguments);
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        throw new Exception("SQL server is not available.", ex);
      }
    }

    public void ExecuteNonQueryStoredProc(string procedureName, params SqlParameter[] arguments)
    {
      try
      {
        this.m_dbConnection.ExecuteNonQueryStoredProc(procedureName, arguments);
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        throw new Exception("SQL server is not available.", ex);
      }
    }

    public void ExecuteNonQueryStoredProcWithTimeout(
      string procedureName,
      TimeSpan timeout,
      params SqlParameter[] arguments)
    {
      try
      {
        this.m_dbConnection.ExecuteNonQueryStoredProcWithTimeout(procedureName, timeout, arguments);
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        throw new Exception("SQL server is not available.", ex);
      }
    }

    public CDbConnectionImpl GetConnectionInternal()
    {
      return this.m_dbConnection;
    }

    void IDisposable.Dispose()
    {
      ((IDisposable) this.m_dbConnection).Dispose();
    }

    public static IDbExecutor MakeExecutor(IDatabaseConfiguration dbCfg)
    {
      return (IDbExecutor) CDbExecutor.Make(dbCfg, TimeSpan.FromSeconds((double) dbCfg.Info.StatementTimeout), false);
    }

    public static IDbExecutor MakeExecutor(
      CDbTransaction transaction,
      IDatabaseConfiguration dbCfg)
    {
      return (IDbExecutor) CDbExecutor.MakeTransactional(transaction, TimeSpan.FromSeconds((double) dbCfg.Info.StatementTimeout), false);
    }
  }
}
