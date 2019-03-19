using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.DBManager;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
  internal class CDbExecutorImpl
  {
    private readonly IDbConnection m_dbConnection;
    private readonly bool m_dbQueryLogging;

    public event EventHandler<CSqlExecErrorEventArgs> ErrorOccured;

    public CDbExecutorImpl(IDbConnection dbConnection, TimeSpan execTimeout, bool dbQueryLogging)
    {
      this.m_dbConnection = dbConnection;
      this.m_dbQueryLogging = dbQueryLogging;
    }

    internal SqlConnection DangerousObtainConnection()
    {
      return this.m_dbConnection.ObtainConnection();
    }

    internal SqlTransaction DangerousObtainTransaction()
    {
      return this.m_dbConnection.Transaction;
    }

    public DataTable ExecuteQuery(
      TimeSpan execTimeout,
      string query,
      params SqlParameter[] args)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql query: [{0}], args: [{1}]", (object) query, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) args));
      bool retryDbOperation = false;
      do
      {
        try
        {
          using (SqlCommand selectCommand = new SqlCommand())
          {
            selectCommand.Transaction = this.m_dbConnection.Transaction;
            selectCommand.Connection = this.m_dbConnection.ObtainConnection();
            selectCommand.CommandText = query;
            selectCommand.Parameters.AddRange(args);
            selectCommand.CommandTimeout = (int) execTimeout.TotalSeconds;
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              DataTable dataTable = new DataTable();
              sqlDataAdapter.Fill(dataTable);
              return dataTable;
            }
          }
        }
        catch (Exception ex)
        {
          this.HandleExeptionOrLogError(ex, out retryDbOperation, "Failed to execute SQL query \"{0}\".", (object) query);
        }
      }
      while (retryDbOperation);
      throw new CAppException("Failed to execute SQL query.", new object[0]);
    }

    public SqlDataReader ExecuteStoredProcAsReader(
      TimeSpan execTimeout,
      string procedureName,
      params SqlParameter[] arguments)
    {
      return this.ExecuteReaderInternal(execTimeout, CommandType.StoredProcedure, procedureName, CommandBehavior.SingleResult, arguments);
    }

    public SqlDataReader ExecuteStoredProcAsReader(
      TimeSpan execTimeout,
      string procedureName,
      CommandBehavior commandBehavior,
      params SqlParameter[] arguments)
    {
      return this.ExecuteReaderInternal(execTimeout, CommandType.StoredProcedure, procedureName, commandBehavior, arguments);
    }

    public SqlDataReader ExecuteQueryAsReader(
      TimeSpan execTimeout,
      string query,
      params SqlParameter[] arguments)
    {
      return this.ExecuteReaderInternal(execTimeout, CommandType.Text, query, CommandBehavior.SingleResult, arguments);
    }

    private SqlDataReader ExecuteReaderInternal(
      TimeSpan execTimeout,
      CommandType cmdType,
      string cmdtext,
      CommandBehavior commandBehavior,
      params SqlParameter[] arguments)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql query: [{0}], args: [{1}]", (object) cmdtext, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) arguments));
      bool retryDbOperation = false;
      do
      {
        try
        {
          using (SqlCommand sqlCommand = new SqlCommand())
          {
            sqlCommand.Transaction = this.m_dbConnection.Transaction;
            sqlCommand.Connection = this.m_dbConnection.ObtainConnection();
            sqlCommand.CommandTimeout = (int) execTimeout.TotalSeconds;
            sqlCommand.CommandText = cmdtext;
            sqlCommand.CommandType = cmdType;
            sqlCommand.Parameters.AddRange(arguments);
            return sqlCommand.ExecuteReader(commandBehavior);
          }
        }
        catch (Exception ex)
        {
          this.HandleExeptionOrLogError(ex, out retryDbOperation, "Failed to execute SQL {0} \"{1}\".", cmdType == CommandType.StoredProcedure ? (object) "stored procedure" : (object) "query", (object) cmdtext);
        }
      }
      while (retryDbOperation);
      throw new CAppException("Failed to execute SQL query.", new object[0]);
    }

    public void ExecuteStoredProc(
      TimeSpan timeout,
      DataTable trgTable,
      string procedureName,
      params SqlParameter[] arguments)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql proc: [{0}], args: [{1}]", (object) procedureName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) arguments));
      bool retryDbOperation = false;
      do
      {
        try
        {
          using (SqlCommand selectCommand = new SqlCommand())
          {
            selectCommand.Transaction = this.m_dbConnection.Transaction;
            selectCommand.Connection = this.m_dbConnection.ObtainConnection();
            selectCommand.CommandTimeout = (int) timeout.TotalSeconds;
            selectCommand.CommandText = procedureName;
            selectCommand.CommandType = CommandType.StoredProcedure;
            selectCommand.Parameters.AddRange(arguments);
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
              sqlDataAdapter.Fill(trgTable);
          }
        }
        catch (Exception ex)
        {
          this.HandleExeptionOrLogError(ex, out retryDbOperation, "Failed to execute SQL stored procedure \"{0}\".", (object) procedureName);
        }
      }
      while (retryDbOperation);
    }

    public void ExecuteNonQueryStoredProc(
      TimeSpan execTimeout,
      string procedureName,
      params SqlParameter[] args)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql proc: [{0}], args: [{1}]", (object) procedureName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) args));
      this.ExecuteNonQueryInternal(execTimeout, CommandType.StoredProcedure, procedureName, args);
    }

    public void ExecuteNonQuery(TimeSpan execTimeout, string statement, SqlParameter[] args)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql query: [{0}], args: [{1}]", (object) statement, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) args));
      this.ExecuteNonQueryInternal(execTimeout, CommandType.Text, statement, args);
    }

    public void ExecuteNonQueryInternal(
      TimeSpan execTimeout,
      CommandType cmdType,
      string cmdText,
      params SqlParameter[] arguments)
    {
      bool retryDbOperation = false;
      do
      {
        try
        {
          using (SqlCommand sqlCommand = new SqlCommand())
          {
            sqlCommand.Transaction = this.m_dbConnection.Transaction;
            sqlCommand.Connection = this.m_dbConnection.ObtainConnection();
            sqlCommand.CommandTimeout = (int) execTimeout.TotalSeconds;
            sqlCommand.CommandText = cmdText;
            sqlCommand.CommandType = cmdType;
            sqlCommand.Parameters.AddRange(arguments);
            sqlCommand.ExecuteNonQuery();
          }
        }
        catch (Exception ex)
        {
          this.HandleExeptionOrLogError(ex, out retryDbOperation, "Failed to execute SQL {0} \"{1}\".", cmdType == CommandType.StoredProcedure ? (object) "stored procedure" : (object) "query", (object) cmdText);
        }
      }
      while (retryDbOperation);
    }

    private object ExecuteScalarInternal(
      CommandType type,
      TimeSpan execTimeout,
      string procedureNameOrText,
      params SqlParameter[] arguments)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql proc: [{0}], args: [{1}]", (object) procedureNameOrText, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) arguments));
      bool retryDbOperation = false;
      do
      {
        try
        {
          using (SqlCommand sqlCommand = new SqlCommand())
          {
            sqlCommand.Transaction = this.m_dbConnection.Transaction;
            sqlCommand.Connection = this.m_dbConnection.ObtainConnection();
            sqlCommand.CommandTimeout = (int) execTimeout.TotalSeconds;
            sqlCommand.CommandText = procedureNameOrText;
            sqlCommand.Parameters.AddRange(arguments);
            sqlCommand.CommandType = type;
            return sqlCommand.ExecuteScalar();
          }
        }
        catch (Exception ex)
        {
          this.HandleExeptionOrLogError(ex, out retryDbOperation, "Failed to execute SQL stored procedure \"{0}\".", (object) procedureNameOrText);
        }
      }
      while (retryDbOperation);
      throw new CAppException("Failed to execute SQL query.", new object[0]);
    }

    public object ExecuteScalar(
      TimeSpan execTimeout,
      string procedureName,
      params SqlParameter[] arguments)
    {
      return this.ExecuteScalarInternal(CommandType.StoredProcedure, execTimeout, procedureName, arguments);
    }

    public object ExecuteQueryScalar(
      TimeSpan execTimeout,
      string text,
      params SqlParameter[] arguments)
    {
      return this.ExecuteScalarInternal(CommandType.Text, execTimeout, text, arguments);
    }

    public void ExecNonQueryWithInfoMessages(
      string spName,
      SqlInfoMessageEventHandler eventHandler,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      if (this.m_dbQueryLogging)
        Log.Information("Sql proc: [{0}], args: [{1}]", (object) spName, (object) CDbAccessor.SqlParamsToString((IEnumerable<SqlParameter>) spParams));
      bool retryDbOperation = false;
      do
      {
        try
        {
          using (SqlCommand sqlCommand = new SqlCommand())
          {
            try
            {
              sqlCommand.Transaction = this.m_dbConnection.Transaction;
              sqlCommand.Connection = this.m_dbConnection.ObtainConnection();
              sqlCommand.Connection.InfoMessage += eventHandler;
              sqlCommand.CommandText = spName;
              sqlCommand.Parameters.AddRange(spParams);
              sqlCommand.CommandType = CommandType.StoredProcedure;
              sqlCommand.CommandTimeout = timeoutSeconds;
              sqlCommand.ExecuteNonQuery();
            }
            finally
            {
              sqlCommand.Connection.InfoMessage -= eventHandler;
            }
          }
        }
        catch (Exception ex)
        {
          this.HandleExeptionOrLogError(ex, out retryDbOperation, "Failed to execute SQL stored procedure \"{0}\".", (object) spName);
        }
      }
      while (retryDbOperation);
    }

    protected void OnErrorOccured(CSqlExecErrorEventArgs args)
    {
      EventHandler<CSqlExecErrorEventArgs> errorOccured = this.ErrorOccured;
      if (errorOccured == null)
        return;
      errorOccured((object) this, args);
    }

    private void HandleExeption(Exception sqlExc, out bool retryDbOperation)
    {
      CSqlExecErrorEventArgs args = new CSqlExecErrorEventArgs(sqlExc);
      this.OnErrorOccured(args);
      retryDbOperation = args.RetryDbOperation;
      if (!retryDbOperation)
        return;
      Log.Information("Retrying SQL operation. Error: [{0}]", (object) CExceptionUtil.GetFirstChanceExc(sqlExc).Message);
    }

    private void HandleExeptionOrLogError(
      Exception sqlExc,
      out bool retryDbOperation,
      string msgFormat,
      params object[] args)
    {
      bool flag = true;
      try
      {
        this.HandleExeption(sqlExc, out retryDbOperation);
        flag = false;
        if (retryDbOperation)
          return;
        CExceptionUtil.RegenTraceExc(sqlExc, msgFormat, args);
      }
      finally
      {
        if (flag)
          Log.Error(msgFormat, args);
      }
    }
  }
}
