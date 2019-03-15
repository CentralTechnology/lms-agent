using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CNotRetryableDbConnection : IDbConnection, IDisposable
  {
    private readonly TimeSpan _connectingTimeout = TimeSpan.FromSeconds(15.0);
    private readonly CDisposableList _disposableObjects = new CDisposableList();
    private readonly SqlConnection _connection;
    private readonly ManualResetEvent _evtConnectionReady;
    private SqlTransaction _sqlTransaction;

    public CNotRetryableDbConnection(string connectionString)
    {
      try
      {
        this._evtConnectionReady = new ManualResetEvent(false);
        this._disposableObjects.Add<ManualResetEvent>(this._evtConnectionReady);
        this._connection = new SqlConnection(connectionString);
        this._disposableObjects.Add<SqlConnection>(this._connection);
        if (!this.DoTryToObtainConnection())
          throw new CAppException("SQL server is not available", new object[0]);
      }
      catch (Exception ex)
      {
        this._disposableObjects.Dispose();
        throw;
      }
    }

    SqlTransaction IDbConnection.Transaction
    {
      get
      {
        return this._sqlTransaction;
      }
    }

    void IDisposable.Dispose()
    {
      try
      {
        this._disposableObjects.Dispose();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Failed to dispose CNotRetryableDbConnection.");
      }
    }

    public void BeginTransaction(string transactionName, IsolationLevel tranLevel)
    {
      this._sqlTransaction = this._connection.BeginTransaction(tranLevel, transactionName);
      this._disposableObjects.Add<SqlTransaction>(this._sqlTransaction);
    }

    SqlConnection IDbConnection.ObtainConnection()
    {
      return this._connection;
    }

    bool IDbConnection.CheckOpenedConnection()
    {
      if (this._connection != null)
      {
        try
        {
          if (SOptions.Instance.SkipSqlConnectionCheck)
            return true;
          SqlCommand command = this._connection.CreateCommand();
          command.CommandText = "SELECT [current_version] FROM [dbo].[Version]";
          command.CommandType = CommandType.Text;
          using (SqlDataReader sqlDataReader = command.ExecuteReader())
          {
            if (sqlDataReader.Read())
              sqlDataReader.GetInt32(0);
          }
          return true;
        }
        catch (Exception ex)
        {
          Log.Information("Checking DB failed. Closing current SQL connection. Error: [{0}]", (object) CExceptionUtil.GetFirstChanceExc(ex).Message);
          this._connection.Close();
        }
      }
      return false;
    }

    bool IDbConnection.TryToObtainConnection()
    {
      if (this._connection == null)
        throw new ObjectDisposedException("CRetryableConnect");
      return this._connection.State == ConnectionState.Open;
    }

    private bool DoTryToObtainConnection()
    {
      this._connection.StateChange += new StateChangeEventHandler(this.ConnectionStateChange);
      try
      {
        this._connection.Open();
        if (!this._evtConnectionReady.WaitOne(this._connectingTimeout, false))
          return false;
        return this._connection.State == ConnectionState.Open;
      }
      finally
      {
        this._connection.StateChange -= new StateChangeEventHandler(this.ConnectionStateChange);
      }
    }

    private void ConnectionStateChange(object sender, StateChangeEventArgs e)
    {
      if (this._connection.State == ConnectionState.Connecting || this._connection.State == ConnectionState.Executing || this._connection.State == ConnectionState.Fetching)
        return;
      this._evtConnectionReady.Set();
    }
  }
}
