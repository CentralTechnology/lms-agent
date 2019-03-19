using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Configuration.V65;
using LMS.Core.Veeam.DBManager;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class LocalDbAccessor : IDatabaseAccessor, IDisposable, ITransactionProvider, IDbAccessor
  {
    public const int Version = 1;
    private readonly string _connectionString;
    private readonly int _commandTimeout;
    private readonly CRetriableQueryExecuter _retriableQueryExecuter;
    private readonly LocalDbAccessor.CTransactionManager _transactionManager;

    public LocalDbAccessor(string connectionString, IDatabaseConfiguration dbConnectionParams)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentNullException(nameof (connectionString));
      this._connectionString = connectionString;
      this._commandTimeout = dbConnectionParams.Info.StatementTimeout;
      this._transactionManager = new LocalDbAccessor.CTransactionManager(connectionString);
      this._retriableQueryExecuter = new CRetriableQueryExecuter((IDatabaseAccessor) this);
    }

    public EDatabaseAccessorType Type
    {
      get
      {
        return EDatabaseAccessorType.Local;
      }
    }

    public int ExecNonQuery(
      CStoredProcedureData procedureData,
      CTransactionScopeIdentifier? identifier)
    {
      return this.ExecNonQuery(CommandType.StoredProcedure, identifier, procedureData.ProcedureName, true, (SqlParameter[]) procedureData.Parameters);
    }

    public int ExecNonQuery(CStoredProcedureData procedureData)
    {
      return this.ExecNonQuery(CommandType.StoredProcedure, procedureData, true);
    }

    public int ExecNonQuery(string spName, params SqlParameter[] spParams)
    {
      return this.ExecNonQuery(CommandType.StoredProcedure, spName, true, spParams);
    }

    public int ExecNonQuery(
      CTransactionScopeIdentifier? identifier,
      string spName,
      params SqlParameter[] spParams)
    {
      return this.ExecNonQuery(CommandType.StoredProcedure, identifier, spName, true, spParams);
    }

    public int ExecNonQuery(
      CommandType type,
      string spName,
      bool retryable,
      params SqlParameter[] spParams)
    {
      return this.ExecNonQuery(type, new CStoredProcedureData(spName, spParams), retryable);
    }

    public int ExecNonQuery(CommandType type, CStoredProcedureData procedureData, bool retryable)
    {
      if (!retryable)
        return this.ExecNonQuery(type, procedureData, new CTransactionScopeIdentifier?(), false);
      return this._retriableQueryExecuter.ExecNonQuery(type, procedureData, new CTransactionScopeIdentifier?());
    }

    public int ExecNonQuery(
      CommandType type,
      CTransactionScopeIdentifier? identifier,
      string spName,
      bool retryable,
      params SqlParameter[] spParams)
    {
      LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(identifier);
      if (sqlTransactionInfo != null)
        retryable = false;
      if (sqlTransactionInfo == null && retryable)
        return this._retriableQueryExecuter.ExecNonQuery(type, new CStoredProcedureData(spName, spParams), identifier);
      return this.ExecNonQuery(type, new CStoredProcedureData(spName, spParams), identifier, false);
    }

    public int ExecNonQuery(
      CommandType type,
      CStoredProcedureData procedureData,
      CTransactionScopeIdentifier? identifier,
      bool retryable)
    {
      try
      {
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(identifier);
        if (sqlTransactionInfo != null)
          retryable = false;
        if (sqlTransactionInfo == null)
        {
          Log.Debug("[DB] {0}, {1}", (object) procedureData.ProcedureName, (object) CDbAccessor.SqlParamsToString(procedureData.Parameters));
          using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
          {
            return this.ExecSqlCommand(type, connection, procedureData);
          }
        }
        else
        {
          Log.Debug("[DB] transactional {0}, {1}", (object) procedureData.ProcedureName, (object) CDbAccessor.SqlParamsToString(procedureData.Parameters));
          return this.ExecSqlCommand(type, sqlTransactionInfo.SqlConnection, procedureData, sqlTransactionInfo.SqlTransaction);
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex, retryable);
        throw;
      }
    }

    private static bool TryParseInfoMessage(string message, out string ourMessage)
    {
      if (message.StartsWith("[DBLOG]", StringComparison.InvariantCultureIgnoreCase))
      {
        ourMessage = message.Substring("[DBLOG]".Length);
        return true;
      }
      ourMessage = (string) null;
      return false;
    }

    public CScalarRetValue ExecScalar(string spName, params SqlParameter[] spParams)
    {
      return this.ExecScalar(spName, true, spParams);
    }

    public CScalarRetValue ExecScalar(
      string spName,
      bool retryable,
      params SqlParameter[] spParams)
    {
      try
      {
        Log.Debug("[DB] {0}, {1}", (object) spName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) spParams));
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(new CTransactionScopeIdentifier?());
        if (sqlTransactionInfo != null)
          retryable = false;
        SqlConnection connection = sqlTransactionInfo == null ? CDbConnection.OpenConnection(this._connectionString) : sqlTransactionInfo.SqlConnection;
        try
        {
          SqlCommand command = sqlTransactionInfo == null ? new SqlCommand(spName, connection) : new SqlCommand(spName, connection, sqlTransactionInfo.SqlTransaction);
          using (command)
          {
            command.Parameters.AddRange(spParams);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = this._commandTimeout;
            return new CScalarRetValue(retryable ? this._retriableQueryExecuter.ExecScalar(command) : command.ExecuteScalar());
          }
        }
        finally
        {
          if (sqlTransactionInfo == null)
            connection.Dispose();
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex, retryable);
        throw;
      }
    }

    public int ExecNonQueryWithReturnValue(string spName, params SqlParameter[] spParams)
    {
      return this.ExecNonQueryWithReturnValue(spName, this._commandTimeout, spParams);
    }

    public int ExecNonQueryWithReturnValue(
      string spName,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      return this.ExecNonQueryWithReturnValue(new CTransactionScopeIdentifier?(), spName, timeoutSeconds, spParams);
    }

    public int ExecNonQueryWithReturnValue(
      CTransactionScopeIdentifier? identifier,
      string spName,
      params SqlParameter[] spParams)
    {
      return this.ExecNonQueryWithReturnValue(identifier, spName, this._commandTimeout, spParams);
    }

    public int ExecNonQueryWithReturnValue(
      CTransactionScopeIdentifier? identifier,
      string spName,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      try
      {
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(identifier);
        if (sqlTransactionInfo != null)
        {
          Log.Debug("[DB] transactional {0}, {1}", (object) spName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) spParams));
          using (SqlCommand sqlCommand = new SqlCommand(spName, sqlTransactionInfo.SqlConnection, sqlTransactionInfo.SqlTransaction))
          {
            sqlCommand.Parameters.AddRange(spParams);
            SqlParameter sqlParameter = sqlCommand.Parameters.Add("RetVal", SqlDbType.Int);
            sqlParameter.Direction = ParameterDirection.ReturnValue;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandTimeout = timeoutSeconds;
            sqlCommand.ExecuteNonQuery();
            return (int) sqlParameter.Value;
          }
        }
        else
        {
          Log.Debug("[DB] {0}, {1}", (object) spName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) spParams));
          using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
          {
            using (SqlCommand sqlCommand = new SqlCommand(spName, connection))
            {
              sqlCommand.Parameters.AddRange(spParams);
              SqlParameter sqlParameter = sqlCommand.Parameters.Add("RetVal", SqlDbType.Int);
              sqlParameter.Direction = ParameterDirection.ReturnValue;
              sqlCommand.CommandType = CommandType.StoredProcedure;
              sqlCommand.CommandTimeout = timeoutSeconds;
              sqlCommand.ExecuteNonQuery();
              return (int) sqlParameter.Value;
            }
          }
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public int ExecNonQueryWithInfoMessages(
      string spName,
      SqlInfoMessageEventHandler eventHandler,
      params SqlParameter[] spParams)
    {
      try
      {
        CStoredProcedureData procedureData = new CStoredProcedureData(spName, spParams);
        Log.Debug("[DB] {0}", (object) procedureData);
        using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
        {
          try
          {
            connection.InfoMessage += eventHandler;
            return this.ExecSqlCommand(connection, procedureData);
          }
          finally
          {
            connection.InfoMessage -= eventHandler;
          }
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public int ExecNonQueryWithInfoMessages(
      string spName,
      SqlInfoMessageEventHandler eventHandler,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      try
      {
        Log.Debug("[DB] {0}", (object) new CStoredProcedureData(spName, spParams));
        using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
        {
          using (SqlCommand sqlCommand = new SqlCommand(spName, connection))
          {
            try
            {
              sqlCommand.Parameters.AddRange(spParams);
              connection.InfoMessage += eventHandler;
              sqlCommand.CommandType = CommandType.StoredProcedure;
              sqlCommand.CommandTimeout = timeoutSeconds;
              return sqlCommand.ExecuteNonQuery();
            }
            finally
            {
              connection.InfoMessage -= eventHandler;
            }
          }
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public int ExecSqlCommand(
      CommandType type,
      SqlCommand command,
      CStoredProcedureData procedureData)
    {
      try
      {
        command.Parameters.AddRange(procedureData.Parameters.ToArray<SqlParameter>());
        command.CommandType = type;
        command.CommandTimeout = procedureData.IsTimeoutSpecified ? procedureData.Timeout : this._commandTimeout;
        return command.ExecuteNonQuery();
      }
      finally
      {
        command.Parameters.Clear();
      }
    }

    public int ExecSqlCommand(SqlCommand command, CStoredProcedureData procedureData)
    {
      return this.ExecSqlCommand(CommandType.StoredProcedure, command, procedureData);
    }

    public int ExecSqlCommand(
      SqlConnection connection,
      CStoredProcedureData procedureData,
      SqlTransaction transaction)
    {
      return this.ExecSqlCommand(CommandType.StoredProcedure, connection, procedureData, transaction);
    }

    public int ExecSqlCommand(
      CommandType type,
      SqlConnection connection,
      CStoredProcedureData procedureData,
      SqlTransaction transaction)
    {
      using (SqlCommand command = new SqlCommand(procedureData.ProcedureName, connection, transaction))
        return this.ExecSqlCommand(type, command, procedureData);
    }

    public int ExecSqlCommand(SqlConnection connection, CStoredProcedureData procedureData)
    {
      return this.ExecSqlCommand(CommandType.StoredProcedure, connection, procedureData);
    }

    public int ExecSqlCommand(
      CommandType type,
      SqlConnection connection,
      CStoredProcedureData procedureData)
    {
      using (SqlCommand command = new SqlCommand(procedureData.ProcedureName, connection))
        return this.ExecSqlCommand(type, command, procedureData);
    }

    public void ExecQuerySequence(
      CTransactionScopeIdentifier? identifier,
      params CStoredProcedureData[] procedureDatas)
    {
      try
      {
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(new CTransactionScopeIdentifier?());
        if (sqlTransactionInfo == null)
        {
          using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
          {
            using (CTransaction ctransaction = this.BeginTransaction(identifier))
            {
              Log.Debug("[DB] Transaction started");
              foreach (CStoredProcedureData procedureData in procedureDatas)
              {
                Log.Debug("[DB] {0}, {1}", (object) procedureData.ProcedureName, (object) CDbAccessor.SqlParamsToString(procedureData.Parameters));
                this.ExecSqlCommand(connection, procedureData);
              }
              ctransaction.Commit();
              Log.Debug("[DB] Transaction committed");
            }
          }
        }
        else
        {
          Log.Debug("[DB] Continuing transaction");
          foreach (CStoredProcedureData procedureData in procedureDatas)
          {
            Log.Debug("[DB] {0}, {1}", (object) procedureData.ProcedureName, (object) CDbAccessor.SqlParamsToString(procedureData.Parameters));
            this.ExecSqlCommand(sqlTransactionInfo.SqlConnection, procedureData, sqlTransactionInfo.SqlTransaction);
          }
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    IDataReader IDbAccessor.ExecQuery(
      string query,
      params SqlParameter[] parameters)
    {
      try
      {
        Log.Debug("[DB] {0}, {1}", (object) query, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) parameters));
        using (SqlCommand sqlCommand = new SqlCommand(query, CDbConnection.OpenConnection(this._connectionString)))
        {
          sqlCommand.Parameters.AddRange(parameters);
          sqlCommand.CommandType = CommandType.Text;
          sqlCommand.CommandTimeout = this._commandTimeout;
          return (IDataReader) sqlCommand.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.CloseConnection);
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public void ExecCommand(CTransactionScopeIdentifier identifier, string commandText)
    {
      try
      {
        Log.Debug("[DB] {0}, {1}", (object) commandText);
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(new CTransactionScopeIdentifier?(identifier));
        Guard.ThrowIfNull((object) sqlTransactionInfo, "Transaction with identifier {0} was not opened before", (object) identifier.Serial());
        using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlTransactionInfo.SqlConnection, sqlTransactionInfo.SqlTransaction))
        {
          sqlCommand.CommandType = CommandType.Text;
          sqlCommand.CommandTimeout = this._commandTimeout;
          sqlCommand.ExecuteNonQuery();
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public IDataReader ExecQuery(
      string query,
      CommandBehavior behavior,
      params SqlParameter[] parameters)
    {
      return this.ExecQuery(query, behavior, this._commandTimeout, parameters);
    }

    public IDataReader ExecQuery(
      string query,
      CommandBehavior behavior,
      int commandTimeout,
      params SqlParameter[] parameters)
    {
      return this.ExecQuery(query, CommandType.StoredProcedure, behavior, commandTimeout, parameters);
    }

    public IDataReader ExecQuery(
      string query,
      CommandType commandType,
      CommandBehavior behavior,
      int commandTimeout,
      params SqlParameter[] parameters)
    {
      try
      {
        Log.Debug("[DB] {0}, {1}", (object) query, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) parameters));
        using (SqlCommand sqlCommand = new SqlCommand(query, CDbConnection.OpenConnection(this._connectionString)))
        {
          sqlCommand.Parameters.AddRange(parameters);
          sqlCommand.CommandType = commandType;
          sqlCommand.CommandTimeout = commandTimeout;
          return (IDataReader) sqlCommand.ExecuteReader(behavior);
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      params SqlParameter[] spParams)
    {
      return this.GetDataTable(spName, commandBehavior, true, spParams);
    }

    public DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      bool retryable,
      params SqlParameter[] spParams)
    {
      if (!retryable)
        return this.GetDataTable(spName, commandBehavior, this._commandTimeout, false, spParams);
      return this._retriableQueryExecuter.GetDataTable(spName, commandBehavior, this._commandTimeout, spParams);
    }

    public DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      bool retryable,
      params SqlParameter[] spParams)
    {
      try
      {
        Log.Debug("[DB] {0}, {1}", (object) spName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) spParams));
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(new CTransactionScopeIdentifier?());
        SqlConnection connection = sqlTransactionInfo == null ? CDbConnection.OpenConnection(this._connectionString) : sqlTransactionInfo.SqlConnection;
        if (sqlTransactionInfo != null)
          retryable = false;
        try
        {
          SqlCommand selectCommand = sqlTransactionInfo == null ? new SqlCommand(spName, connection) : new SqlCommand(spName, connection, sqlTransactionInfo.SqlTransaction);
          DataTable dataTable = new DataTable();
          using (selectCommand)
          {
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              try
              {
                selectCommand.Parameters.AddRange(spParams);
                selectCommand.CommandType = CommandType.StoredProcedure;
                selectCommand.CommandTimeout = timeoutSeconds;
                dataTable.BeginLoadData();
                sqlDataAdapter.Fill(dataTable);
                dataTable.EndLoadData();
              }
              finally
              {
                selectCommand.Parameters.Clear();
              }
            }
          }
          return dataTable;
        }
        finally
        {
          if (sqlTransactionInfo == null)
            connection.Dispose();
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex, retryable);
        throw;
      }
    }

    public DataTable GetDataTable(
      string query,
      CommandType type,
      params SqlParameter[] spParams)
    {
      try
      {
        Log.Debug("[DB] {0}", (object) query);
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this._transactionManager.GetSqlTransactionInfo(new CTransactionScopeIdentifier?());
        SqlConnection connection = sqlTransactionInfo == null ? CDbConnection.OpenConnection(this._connectionString) : sqlTransactionInfo.SqlConnection;
        try
        {
          SqlCommand selectCommand = sqlTransactionInfo == null ? new SqlCommand(query, connection) : new SqlCommand(query, connection, sqlTransactionInfo.SqlTransaction);
          DataTable dataTable = new DataTable();
          using (selectCommand)
          {
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              selectCommand.CommandType = type;
              selectCommand.CommandTimeout = this._commandTimeout;
              selectCommand.Parameters.AddRange(spParams);
              dataTable.BeginLoadData();
              sqlDataAdapter.Fill(dataTable);
              dataTable.EndLoadData();
            }
          }
          return dataTable;
        }
        finally
        {
          if (sqlTransactionInfo == null)
            connection.Dispose();
        }
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public DataTable GetDataTable(string spName, params SqlParameter[] spParams)
    {
      return this.GetDataTable(spName, true, spParams);
    }

    public DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      return this.GetDataTable(spName, commandBehavior, timeoutSeconds, true, spParams);
    }

    public DataTable GetDataTable(
      string spName,
      bool retryable,
      params SqlParameter[] spParams)
    {
      return this.GetDataTable(spName, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, retryable, spParams);
    }

    public DataTable GetDataTable(
      string spName,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      return this.GetDataTable(spName, timeoutSeconds, true, spParams);
    }

    public DataTable GetDataTable(
      string spName,
      int timeoutSeconds,
      bool retryable,
      params SqlParameter[] spParams)
    {
      if (!retryable)
        return this.GetDataTable(spName, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, timeoutSeconds, false, spParams);
      return this._retriableQueryExecuter.GetDataTable(spName, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, timeoutSeconds, spParams);
    }

    public DataSet GetDataSet(
      string spName,
      CommandBehavior commandBehavior,
      params SqlParameter[] spParams)
    {
      return this.GetDataSet(spName, commandBehavior, this._commandTimeout, spParams);
    }

    public DataSet GetDataSet(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      try
      {
        Log.Debug("[DB] {0}, {1}", (object) spName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) spParams));
        DataSet dataSet = new DataSet();
        using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
        {
          using (SqlCommand selectCommand = new SqlCommand(spName, connection))
          {
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              selectCommand.Parameters.AddRange(spParams);
              selectCommand.CommandType = CommandType.StoredProcedure;
              selectCommand.CommandTimeout = timeoutSeconds;
              sqlDataAdapter.Fill(dataSet);
            }
          }
        }
        return dataSet;
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public DataSet GetDataSet(string query, CommandType type)
    {
      try
      {
        Log.Debug("[DB] {0}", (object) query);
        DataSet dataSet = new DataSet();
        using (SqlConnection connection = CDbConnection.OpenConnection(this._connectionString))
        {
          using (SqlCommand selectCommand = new SqlCommand(query, connection))
          {
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              selectCommand.CommandType = type;
              selectCommand.CommandTimeout = this._commandTimeout;
              sqlDataAdapter.Fill(dataSet);
            }
          }
        }
        return dataSet;
      }
      catch (Exception ex)
      {
        CExceptionUtil.ThrowSqlException(ex);
        throw;
      }
    }

    public CTransaction BeginTransaction(CTransactionScopeIdentifier? identifier)
    {
      CTransaction transaction = new CTransaction(identifier ?? CTransactionScopeIdentifier.CreateCurrent(), (ITransactionProvider) this);
      this._transactionManager.CreateSqlTransaction(transaction);
      return transaction;
    }

    public CTransaction BeginTransactionAtSnapshotIsolationLevel(
      CTransactionScopeIdentifier? identifier)
    {
      CTransaction transaction = new CTransaction(identifier ?? CTransactionScopeIdentifier.CreateCurrent(), (ITransactionProvider) this);
      try
      {
        this._transactionManager.CreateSqlTransaction(transaction, "SET TRANSACTION ISOLATION LEVEL SNAPSHOT;");
      }
      catch (CSqlException ex)
      {
        Log.Error((Exception) ex, (string) null);
        this._transactionManager.CreateSqlTransaction(transaction, (string) null);
      }
      return transaction;
    }

    public bool IsRemote
    {
      get
      {
        return false;
      }
    }

    public void CommitTransaction(CTransactionScopeIdentifier identifier)
    {
      ((ITransactionProvider) this).CommitTransaction(identifier);
    }

    void ITransactionProvider.CommitTransaction(
      CTransactionScopeIdentifier identifier)
    {
      this._transactionManager.CommitSqlTransaction(identifier);
    }

    void ITransactionProvider.RollbackTransaction(
      CTransactionScopeIdentifier identifier)
    {
      this._transactionManager.RollbackSqlTransaction(identifier);
    }

    public void RollbackTransaction(CTransactionScopeIdentifier identifier)
    {
      ((ITransactionProvider) this).CommitTransaction(identifier);
    }

    public void Dispose()
    {
      this._transactionManager.Dispose();
    }

    private class CSqlTransactionInfo
    {
      private readonly CTransactionScopeIdentifier _identifier;
      private readonly SqlConnection _sqlConnection;
      private readonly SqlTransaction _sqlTransaction;

      public CSqlTransactionInfo(
        CTransactionScopeIdentifier identifier,
        SqlConnection sqlConnection,
        SqlTransaction sqlTransaction)
      {
        this._identifier = identifier;
        this._sqlConnection = sqlConnection;
        this._sqlTransaction = sqlTransaction;
      }

      public CTransactionScopeIdentifier Identifier
      {
        get
        {
          return this._identifier;
        }
      }

      public SqlConnection SqlConnection
      {
        get
        {
          return this._sqlConnection;
        }
      }

      public SqlTransaction SqlTransaction
      {
        get
        {
          return this._sqlTransaction;
        }
      }
    }

    private class CTransactionManager : IDisposable
    {
      private readonly ConcurrentCache<CTransactionScopeIdentifier, Stack<LocalDbAccessor.CSqlTransactionInfo>> _cache;
      private readonly string _connectionString;

      public CTransactionManager(string connectionString)
      {
        this._cache = ConcurrentCache.Create<CTransactionScopeIdentifier, Stack<LocalDbAccessor.CSqlTransactionInfo>>((ConcurrentCacheFactory<CTransactionScopeIdentifier, Stack<LocalDbAccessor.CSqlTransactionInfo>>) new LocalDbAccessor.CTransactionManager.CacheFactory());
        this._cache.ReleaseImmediately = true;
        this._connectionString = connectionString;
      }

      public void Dispose()
      {
        this.CommitUncomiitedTransactions();
        this._cache.Dispose();
      }

      public void CreateSqlTransaction(CTransaction transaction)
      {
        this.CreateSqlTransaction(transaction, (string) null);
      }

      public void CreateSqlTransaction(CTransaction transaction, string preOpenTransactionCommands)
      {
        Log.Information("[DbAccessor] Start transaction.");
        Stack<LocalDbAccessor.CSqlTransactionInfo> csqlTransactionInfoStack = this._cache.Acquire(transaction.Identifier);
        try
        {
          SqlConnection sqlConnection = CDbConnection.OpenConnection(this._connectionString);
          if (!string.IsNullOrEmpty(preOpenTransactionCommands))
          {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = preOpenTransactionCommands;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
          }
          SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
          LocalDbAccessor.CSqlTransactionInfo csqlTransactionInfo = new LocalDbAccessor.CSqlTransactionInfo(transaction.Identifier, sqlConnection, sqlTransaction);
          lock (csqlTransactionInfoStack)
            csqlTransactionInfoStack.Push(csqlTransactionInfo);
        }
        catch
        {
          this._cache.Release(transaction.Identifier);
          throw;
        }
      }

      private LocalDbAccessor.CSqlTransactionInfo PopTransaction(
        CTransactionScopeIdentifier identifier)
      {
        Stack<LocalDbAccessor.CSqlTransactionInfo> csqlTransactionInfoStack = this._cache.Release(identifier);
        lock (csqlTransactionInfoStack)
          return csqlTransactionInfoStack.Pop();
      }

      private static void DisposeTransaction(
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo)
      {
        Log.Information("[DbAccessor] Dispose transaction.");
        sqlTransactionInfo.SqlTransaction.Dispose();
        sqlTransactionInfo.SqlConnection.Dispose();
      }

      public void CommitSqlTransaction(CTransactionScopeIdentifier identifier)
      {
        Log.Information("[DbAccessor] Commit transaction.");
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this.PopTransaction(identifier);
        sqlTransactionInfo.SqlTransaction.Commit();
        LocalDbAccessor.CTransactionManager.DisposeTransaction(sqlTransactionInfo);
      }

      public void RollbackSqlTransaction(CTransactionScopeIdentifier identifier)
      {
        LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = this.PopTransaction(identifier);
        sqlTransactionInfo.SqlTransaction.Rollback();
        LocalDbAccessor.CTransactionManager.DisposeTransaction(sqlTransactionInfo);
      }

      public LocalDbAccessor.CSqlTransactionInfo GetSqlTransactionInfo(
        CTransactionScopeIdentifier? identifier = null)
      {
        return this.GetSqlTransactionInfoImpl(identifier ?? CTransactionScopeIdentifier.CreateCurrent());
      }

      private LocalDbAccessor.CSqlTransactionInfo GetSqlTransactionInfoImpl(
        CTransactionScopeIdentifier identifier)
      {
        Stack<LocalDbAccessor.CSqlTransactionInfo> csqlTransactionInfoStack = this._cache.PeekIfExists(identifier);
        if (csqlTransactionInfoStack == null)
          return (LocalDbAccessor.CSqlTransactionInfo) null;
        lock (csqlTransactionInfoStack)
          return csqlTransactionInfoStack.Count == 0 ? (LocalDbAccessor.CSqlTransactionInfo) null : csqlTransactionInfoStack.Peek();
      }

      private void CommitUncomiitedTransactions()
      {
        foreach (KeyValuePair<CTransactionScopeIdentifier, Stack<LocalDbAccessor.CSqlTransactionInfo>> asPair in this._cache.AsPairs())
          LocalDbAccessor.CTransactionManager.CommitTransactions(asPair.Value);
      }

      private static void CommitTransactions(Stack<LocalDbAccessor.CSqlTransactionInfo> stack)
      {
        try
        {
          lock (stack)
          {
            while (stack.Count > 0)
            {
              LocalDbAccessor.CSqlTransactionInfo sqlTransactionInfo = stack.Pop();
              sqlTransactionInfo.SqlTransaction.Commit();
              LocalDbAccessor.CTransactionManager.DisposeTransaction(sqlTransactionInfo);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Faield to dispose a transaction scope.");
        }
      }

      private class CacheFactory : ConcurrentCacheFactory<CTransactionScopeIdentifier, Stack<LocalDbAccessor.CSqlTransactionInfo>>
      {
        public override Stack<LocalDbAccessor.CSqlTransactionInfo> Create(
          CTransactionScopeIdentifier key)
        {
          return new Stack<LocalDbAccessor.CSqlTransactionInfo>();
        }

        public override void Destroy(
          CTransactionScopeIdentifier key,
          Stack<LocalDbAccessor.CSqlTransactionInfo> stack,
          bool cacheDisposing)
        {
        }
      }
    }
  }
}
