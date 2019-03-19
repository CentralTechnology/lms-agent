using System;
using System.Data;
using System.Data.SqlClient;

namespace LMS.Core.Veeam.Backup.Common
{
  public interface IDatabaseAccessor : IDisposable, ITransactionProvider, IDbAccessor
  {
    EDatabaseAccessorType Type { get; }

    int ExecNonQuery(CStoredProcedureData procedureData, CTransactionScopeIdentifier? identifier);

    int ExecNonQuery(CStoredProcedureData procedureData);

    int ExecNonQuery(string spName, params SqlParameter[] spParams);

    int ExecNonQuery(
      CTransactionScopeIdentifier? identifier,
      string spName,
      params SqlParameter[] spParams);

    int ExecNonQuery(
      CommandType type,
      CStoredProcedureData procedureData,
      CTransactionScopeIdentifier? identifier,
      bool retryable);

    int ExecNonQuery(CommandType type, CStoredProcedureData procedureData, bool retryable);

    int ExecNonQuery(
      CommandType type,
      string spName,
      bool retryable,
      params SqlParameter[] spParams);

    int ExecNonQuery(
      CommandType type,
      CTransactionScopeIdentifier? identifier,
      string spName,
      bool retryable,
      params SqlParameter[] spParams);

    CScalarRetValue ExecScalar(string spName, params SqlParameter[] spParams);

    CScalarRetValue ExecScalar(
      string spName,
      bool retryable,
      params SqlParameter[] spParams);

    DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      params SqlParameter[] spParams);

    DataTable GetDataTable(string spName, params SqlParameter[] spParams);

    DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      params SqlParameter[] spParams);

    DataTable GetDataTable(
      string spName,
      int timeoutSeconds,
      params SqlParameter[] spParams);

    DataTable GetDataTable(string query, CommandType type, params SqlParameter[] spParams);

    DataTable GetDataTable(string spName, bool retryable, params SqlParameter[] spParams);

    DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      bool retryable,
      params SqlParameter[] spParams);

    DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      bool retryable,
      params SqlParameter[] spParams);

    DataTable GetDataTable(
      string spName,
      int timeoutSeconds,
      bool retryable,
      params SqlParameter[] spParams);

    DataSet GetDataSet(
      string spName,
      CommandBehavior commandBehavior,
      params SqlParameter[] spParams);

    DataSet GetDataSet(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      params SqlParameter[] spParams);

    DataSet GetDataSet(string query, CommandType type);

    int ExecNonQueryWithReturnValue(string spName, params SqlParameter[] spParams);

    int ExecNonQueryWithReturnValue(
      string spName,
      int timeoutSeconds,
      params SqlParameter[] spParams);

    int ExecNonQueryWithReturnValue(
      CTransactionScopeIdentifier? identifier,
      string spName,
      params SqlParameter[] spParams);

    int ExecNonQueryWithReturnValue(
      CTransactionScopeIdentifier? identifier,
      string spName,
      int timeoutSeconds,
      params SqlParameter[] spParams);

    int ExecNonQueryWithInfoMessages(
      string spName,
      SqlInfoMessageEventHandler eventHandler,
      params SqlParameter[] spParams);

    int ExecNonQueryWithInfoMessages(
      string spName,
      SqlInfoMessageEventHandler eventHandler,
      int timeoutSeconds,
      params SqlParameter[] spParams);

    int ExecSqlCommand(SqlCommand command, CStoredProcedureData procedureData);

    int ExecSqlCommand(
      SqlConnection connection,
      CStoredProcedureData procedureData,
      SqlTransaction transaction);

    int ExecSqlCommand(SqlConnection connection, CStoredProcedureData procedureData);

    void ExecQuerySequence(
      CTransactionScopeIdentifier? identifier,
      params CStoredProcedureData[] procedureDatas);

    CTransaction BeginTransactionAtSnapshotIsolationLevel(
      CTransactionScopeIdentifier? identifier = null);

    bool IsRemote { get; }
  }
}
