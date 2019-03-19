using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Configuration.V65;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CDbExecutor : IDbExecutor, IDisposable
  {
    private CDisposableList m_disposableObjects = new CDisposableList();
    private CDbExecutorImpl m_dbExecutor;
    private TimeSpan m_defExecTimeout;

    private CDbExecutor(IDbConnection dbConnection, TimeSpan timeoutExecSql, bool dbQueryLogging)
    {
      this.m_defExecTimeout = timeoutExecSql;
      this.m_dbExecutor = new CDbExecutorImpl(dbConnection, timeoutExecSql, dbQueryLogging);
    }

    private CDbExecutor(
      CAutoRef<IDbConnection> dbConnectionRef,
      TimeSpan timeoutExecSql,
      bool dbQueryLogging)
      : this(dbConnectionRef.Get(), timeoutExecSql, dbQueryLogging)
    {
      this.m_disposableObjects.Add<IDbConnection>(dbConnectionRef.Get());
    }

    public SqlConnection DangerousObtainConnection()
    {
      return this.m_dbExecutor.DangerousObtainConnection();
    }

    public static CDbExecutor Make(
      IDatabaseConfiguration dbConnectionParams,
      TimeSpan timeoutExecSql,
      bool dbQueryLogging)
    {
      return CDbExecutor.Make(dbConnectionParams, timeoutExecSql, dbQueryLogging, 1);
    }

    public static CDbExecutor Make(
      IDatabaseConfiguration dbConnectionParams,
      TimeSpan timeoutExecSql,
      bool dbQueryLogging,
      int attemptsCount)
    {
      using (CAutoRefScope scope = new CAutoRefScope())
      {
        CRetryableConnect cretryableConnect = CRetryableConnect.Do(dbConnectionParams.ConnectionString, attemptsCount);
        CAutoRef<IDbConnection> dbConnectionRef = new CAutoRef<IDbConnection>(scope, (IDbConnection) cretryableConnect);
        return scope.Commit<CDbExecutor>(new CDbExecutor(dbConnectionRef, timeoutExecSql, dbQueryLogging));
      }
    }

    public static CDbExecutor MakeTransactional(
      CDbTransaction transaction,
      TimeSpan timeoutExecSql,
      bool dbQueryLogging)
    {
      return new CDbExecutor(transaction.DbConnection, timeoutExecSql, dbQueryLogging);
    }

    public event EventHandler<CSqlExecErrorEventArgs> ErrorOccured
    {
      add
      {
        this.m_dbExecutor.ErrorOccured += value;
      }
      remove
      {
        this.m_dbExecutor.ErrorOccured -= value;
      }
    }

    void IDisposable.Dispose()
    {
      this.m_disposableObjects.Dispose();
    }

    public DataTable ExecuteQuery(string query, params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteQuery(this.m_defExecTimeout, query, args);
    }

    public SqlDataReader ExecuteQueryAsReader(string query, params SqlParameter[] args)
    {
      return this.ExecuteQueryAsReader(this.m_defExecTimeout, query, args);
    }

    public SqlDataReader ExecuteQueryAsReader(
      TimeSpan timeout,
      string query,
      params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteQueryAsReader(timeout, query, args);
    }

    public DataTable ExecuteStoredProc(string procedureName, params SqlParameter[] args)
    {
      return this.ExecuteStoredProc(this.m_defExecTimeout, procedureName, args);
    }

    public DataTable ExecuteStoredProc(
      TimeSpan timeout,
      string procedureName,
      params SqlParameter[] args)
    {
      DataTable trgTable = new DataTable();
      this.m_dbExecutor.ExecuteStoredProc(timeout, trgTable, procedureName, args);
      return trgTable;
    }

    public SqlDataReader ExecuteStoredProcAsReader(
      string procedureName,
      params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteStoredProcAsReader(this.m_defExecTimeout, procedureName, args);
    }

    public SqlDataReader ExecuteStoredProcAsReader(
      string procedureName,
      CommandBehavior commandBehavior,
      params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteStoredProcAsReader(this.m_defExecTimeout, procedureName, commandBehavior, args);
    }

    public SqlDataReader ExecuteStoredProcAsReader(
      TimeSpan timeout,
      string procedureName,
      params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteStoredProcAsReader(timeout, procedureName, args);
    }

    public SqlDataReader ExecuteStoredProcAsReader(
      int timeoutSeconds,
      string procedureName,
      params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteStoredProcAsReader(TimeSpan.FromSeconds((double) timeoutSeconds), procedureName, args);
    }

    public void ExecuteStoredProc(
      DataTable trgTable,
      string procedureName,
      params SqlParameter[] args)
    {
      this.m_dbExecutor.ExecuteStoredProc(this.m_defExecTimeout, trgTable, procedureName, args);
    }

    public void ExecuteStoredProc(
      TimeSpan timeout,
      DataTable trgTable,
      string procedureName,
      params SqlParameter[] args)
    {
      this.m_dbExecutor.ExecuteStoredProc(timeout, trgTable, procedureName, args);
    }

    public void ExecuteNonQueryStoredProc(string procedureName, params SqlParameter[] args)
    {
      this.ExecuteNonQueryStoredProc(this.m_defExecTimeout, procedureName, args);
    }

    public void ExecuteNonQueryStoredProc(
      TimeSpan timeout,
      string procedureName,
      params SqlParameter[] args)
    {
      this.m_dbExecutor.ExecuteNonQueryStoredProc(timeout, procedureName, args);
    }

    public void ExecuteNonQuery(string statement, params SqlParameter[] args)
    {
      this.ExecuteNonQuery(this.m_defExecTimeout, statement, args);
    }

    public void ExecuteNonQuery(TimeSpan timeout, string statement, params SqlParameter[] args)
    {
      this.m_dbExecutor.ExecuteNonQuery(timeout, statement, args);
    }

    public object ExecuteScalar(string procedureName, params SqlParameter[] args)
    {
      return this.m_dbExecutor.ExecuteScalar(this.m_defExecTimeout, procedureName, args);
    }

    public void ExecuteBulkCopy(IDataReader dataReader, string destinationTable)
    {
      using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(this.m_dbExecutor.DangerousObtainConnection()))
      {
        sqlBulkCopy.DestinationTableName = destinationTable;
        sqlBulkCopy.BulkCopyTimeout = 0;
        sqlBulkCopy.WriteToServer(dataReader);
      }
    }

    public void TransactionalBulkCopy(
      IDataReader dataReader,
      string destinationTable,
      SqlBulkCopyOptions sqlBulkOptions)
    {
      using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(this.m_dbExecutor.DangerousObtainConnection(), sqlBulkOptions, this.m_dbExecutor.DangerousObtainTransaction()))
      {
        sqlBulkCopy.DestinationTableName = destinationTable;
        sqlBulkCopy.BulkCopyTimeout = 0;
        sqlBulkCopy.WriteToServer(dataReader);
      }
    }

    public void ExecNonQueryWithInfoMessages(
      string spName,
      SqlInfoMessageEventHandler eventHandler,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      this.m_dbExecutor.ExecNonQueryWithInfoMessages(spName, eventHandler, timeoutSeconds, spParams);
    }

    public object ExecuteQueryScalar(string text)
    {
      return this.m_dbExecutor.ExecuteQueryScalar(this.m_defExecTimeout, text);
    }
  }
}
