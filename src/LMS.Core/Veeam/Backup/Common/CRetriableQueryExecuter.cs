using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public class CRetriableQueryExecuter
  {
    private static readonly int DeadlockMaxRetryNumber = SOptions.Instance.DatabaseDeadlockRetryNumber;
    private static readonly TimeSpan DeadlockRetrySleepTime = TimeSpan.FromSeconds((double) SOptions.Instance.DatabaseDeadlockRetrySleepTime);
    private static readonly int DatabaseTimeoutRetryNumber = SOptions.Instance.DatabaseTimeoutRetryNumber;
    private static readonly TimeSpan DatabaseTimeoutRetrySleepTime = TimeSpan.FromSeconds((double) SOptions.Instance.DatabaseTimeoutRetrySleepTime);
    private static readonly int DatabaseBrokenConnectionRetryNumber = SOptions.Instance.DatabaseBrokenConnectionRetryNumber;
    private static readonly TimeSpan DatabaseBrokenConnectionRetrySleepTime = TimeSpan.FromSeconds((double) SOptions.Instance.DatabaseBrokenConnectionRetrySleepTime);
    private static readonly int XmlExceptionRetryNumber = SOptions.Instance.XmlExceptionRetryNumber;
    private static readonly TimeSpan XmlExceptionRetrySleepTime = TimeSpan.FromSeconds((double) SOptions.Instance.XmlExceptionRetrySleepTime);
    private static readonly int DatabaseGenericRetryNumber = SOptions.Instance.DatabaseGenericRetryNumber;
    private static readonly TimeSpan DatabaseGenericRetrySleepTime = TimeSpan.FromSeconds((double) SOptions.Instance.DatabaseGenericRetrySleepTime);
    private readonly IDatabaseAccessor _databaseAccessor;

    public CRetriableQueryExecuter(IDatabaseAccessor databaseAccessor)
    {
      this._databaseAccessor = databaseAccessor;
    }

    public DataTable GetDataTable(
      string spName,
      CommandBehavior commandBehavior,
      int timeoutSeconds,
      params SqlParameter[] spParams)
    {
      int retryNumber = 0;
      while (true)
      {
        try
        {
          return this._databaseAccessor.GetDataTable(spName, commandBehavior, timeoutSeconds, true, spParams);
        }
        catch (Exception ex)
        {
          retryNumber = CRetriableQueryExecuter.OnDatabaseRetry(ex, retryNumber, spName);
        }
      }
    }

    public int ExecNonQuery(
      CommandType type,
      CStoredProcedureData spData,
      CTransactionScopeIdentifier? identifier)
    {
      int retryNumber = 0;
      while (true)
      {
        try
        {
          return this._databaseAccessor.ExecNonQuery(type, spData, identifier, true);
        }
        catch (Exception ex)
        {
          retryNumber = CRetriableQueryExecuter.OnDatabaseRetry(ex, retryNumber, spData.ProcedureName);
        }
      }
    }

    public object ExecScalar(SqlCommand command)
    {
      int retryNumber = 0;
      while (true)
      {
        try
        {
          return command.ExecuteScalar();
        }
        catch (Exception ex)
        {
          retryNumber = CRetriableQueryExecuter.OnDatabaseRetry(ex, retryNumber, command.CommandText);
        }
      }
    }

    private static int OnDatabaseRetry(Exception ex, int retryNumber, string spName)
    {
      XmlException initialException1 = CExceptionUtil.FindInitialException<XmlException>(ex);
      if (initialException1 != null)
        return CRetriableQueryExecuter.OnXmlException(initialException1, retryNumber, spName);
      SqlException initialException2 = CExceptionUtil.FindInitialException<SqlException>(ex);
      if (initialException2 == null || !initialException2.Errors.Cast<SqlError>().Any<SqlError>((Func<SqlError, bool>) (x => Enum.IsDefined(typeof (ERetryableDatabaseError), (object) x.Number))))
      {
        Log.Information("[DBRETRY] Nonretryable error has happened. Proc: [{0}]. Error: [{1}]", (object) spName, (object) ex.Message);
        CExceptionUtil.ThrowSqlException(ex);
      }
      ERetryableDatabaseError number = (ERetryableDatabaseError) initialException2.Number;
      int retryNumberByError = CRetriableQueryExecuter.GetDatabaseRetryNumberByError(number);
      TimeSpan sleepTimeByError = CRetriableQueryExecuter.GetDatabaseRetrySleepTimeByError(number);
      if (retryNumber >= retryNumberByError)
        CExceptionUtil.ThrowSqlException((Exception) initialException2);
      Log.Warning("[DBRETRY] {0} error has occured. Affected stored procedure is {1}. Pausing for {2} seconds before retrying", (object) number, (object) spName, (object) sleepTimeByError.TotalSeconds);
      Thread.Sleep(sleepTimeByError);
      ++retryNumber;
      Log.Warning("[DBRETRY] Retrying stored procedure [{0}], attempt number {1} out of {2} attempts", (object) spName, (object) retryNumber, (object) retryNumberByError);
      return retryNumber;
    }

    private static int OnXmlException(XmlException xmlex, int retryNumber, string spName)
    {
      if (retryNumber > CRetriableQueryExecuter.XmlExceptionRetryNumber)
        CExceptionUtil.ThrowSqlException((Exception) xmlex);
      Log.Warning("[DBRETRY] {0} error has occured. Pausing for {1} seconds before retrying", (object) xmlex.ToString(), (object) CRetriableQueryExecuter.XmlExceptionRetrySleepTime);
      Thread.Sleep(CRetriableQueryExecuter.XmlExceptionRetrySleepTime);
      Log.Warning("[DBRETRY] Retrying stored procedure [{0}], attempt number {1} out of {2} attempts", (object) spName, (object) retryNumber, (object) CRetriableQueryExecuter.XmlExceptionRetryNumber);
      return retryNumber;
    }

    private static int GetDatabaseRetryNumberByError(ERetryableDatabaseError error)
    {
      switch (error)
      {
        case ERetryableDatabaseError.SqlTimeout:
          return CRetriableQueryExecuter.DatabaseTimeoutRetryNumber;
        case ERetryableDatabaseError.SqlConnectionBroken:
          return CRetriableQueryExecuter.DatabaseBrokenConnectionRetryNumber;
        case ERetryableDatabaseError.SqlDeadlock:
          return CRetriableQueryExecuter.DeadlockMaxRetryNumber;
        default:
          return CRetriableQueryExecuter.DatabaseGenericRetryNumber;
      }
    }

    private static TimeSpan GetDatabaseRetrySleepTimeByError(ERetryableDatabaseError error)
    {
      switch (error)
      {
        case ERetryableDatabaseError.SqlTimeout:
          return CRetriableQueryExecuter.DatabaseTimeoutRetrySleepTime;
        case ERetryableDatabaseError.SqlConnectionBroken:
          return CRetriableQueryExecuter.DatabaseBrokenConnectionRetrySleepTime;
        case ERetryableDatabaseError.SqlDeadlock:
          return CRetriableQueryExecuter.DeadlockRetrySleepTime;
        default:
          return CRetriableQueryExecuter.DatabaseGenericRetrySleepTime;
      }
    }
  }
}
