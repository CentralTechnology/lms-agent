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
  public class CRetryableConnect : IDbConnection, IDisposable
  {
    private int AttemptsCount = 2;
    private readonly ManualResetEvent _evtConnectionReady = new ManualResetEvent(false);
    private readonly TimeSpan _attemptsDelay = TimeSpan.FromSeconds(2.0);
    private readonly TimeSpan _connectingTimeout = TimeSpan.FromSeconds(10.0);
    private readonly SqlConnection _connection;
    private bool _disposed;

    private CRetryableConnect(string connectionString, int attemptsCount)
    {
      this.AttemptsCount = attemptsCount;
      this._connection = new SqlConnection(connectionString);
    }

    public static CRetryableConnect Do(string connectionString, int attemptsCount)
    {
      return new CRetryableConnect(connectionString, attemptsCount);
    }

    SqlTransaction IDbConnection.Transaction
    {
      get
      {
        return (SqlTransaction) null;
      }
    }

    void IDisposable.Dispose()
    {
      try
      {
        if (this._disposed)
          return;
        this._disposed = true;
        this._connection.Dispose();
        this._evtConnectionReady.Close();
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
      }
    }

    public SqlConnection ObtainConnection()
    {
      if (this._disposed)
        throw new ObjectDisposedException(nameof (CRetryableConnect));
      if (!this.TryToObtainConnection())
        throw new CAppException("SQL server is not available", new object[0]);
      return this._connection;
    }

    public bool CheckOpenedConnection()
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
      }
      catch (Exception ex)
      {
        Log.Information("Checking DB failed. Closing current SQL connection. Error: [{0}]", (object) CExceptionUtil.GetFirstChanceExc(ex).Message);
        this._connection.Close();
        return false;
      }
      return true;
    }

    public bool TryToObtainConnection()
    {
      if (this._disposed)
        throw new ObjectDisposedException(nameof (CRetryableConnect));
      if (this._connection.State != ConnectionState.Open)
      {
        for (int index = 0; index < this.AttemptsCount; ++index)
        {
          try
          {
            if (this.DoTryToObtainConnection())
            {
              if (this.CheckOpenedConnection())
                return true;
            }
          }
          catch (Exception ex)
          {
            if (index == this.AttemptsCount - 1)
              Log.Error(ex, (string) null);
          }
          if (index != this.AttemptsCount - 1)
            Thread.Sleep(this._attemptsDelay);
          else
            break;
        }
        Log.Information("Connection state: [{0}].", (object) this._connection.State);
      }
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
