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
  public class CDbConnection
  {
    private readonly ManualResetEvent _connectionReady = new ManualResetEvent(false);
    private const int AttemptsCount = 2;
    private const int AttemptsDelay = 3000;
    private const int ConnectingTimeout = 15000;
    private readonly string _connectionString;

    public CDbConnection(string connectionString)
    {
      this._connectionString = connectionString;
    }

    public static SqlConnection OpenConnection(string connectionString)
    {
      return new CDbConnection(connectionString).ObtainConnection();
    }

    public SqlConnection ObtainConnection()
    {
      SqlConnection sqlConnection = new SqlConnection(this._connectionString);
      try
      {
        for (int index = 0; index < 2; ++index)
        {
          try
          {
            if (this.DoTryToObtainConnection(sqlConnection))
            {
              if (this.CheckOpenedConnection(sqlConnection))
                return sqlConnection;
            }
          }
          catch (Exception ex)
          {
            Log.Warning("Failed to obtain connection: [{0}]. Attempt {1} of {2}", (object) ex.Message, (object) (index + 1), (object) 2);
            if (index == 1)
              Log.Error(ex, (string) null);
          }
          Thread.Sleep(3000);
        }
        Log.Information("Connection state: [{0}].", (object) sqlConnection.State);
        throw new CAppException("SQL server is not available", new object[0]);
      }
      catch (Exception ex)
      {
        sqlConnection.SafeDispose();
        throw;
      }
    }

    private bool DoTryToObtainConnection(SqlConnection connection)
    {
      try
      {
        this._connectionReady.Reset();
        connection.StateChange += new StateChangeEventHandler(this.ConnectionStateChange);
        connection.Open();
        if (!this._connectionReady.WaitOne(15000, false))
          return false;
        return connection.State == ConnectionState.Open;
      }
      finally
      {
        connection.StateChange -= new StateChangeEventHandler(this.ConnectionStateChange);
      }
    }

    private bool CheckOpenedConnection(SqlConnection connection)
    {
      try
      {
        if (SOptions.Instance.SkipSqlConnectionCheck)
          return true;
        SqlCommand command = connection.CreateCommand();
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
        connection.Close();
        return false;
      }
      return true;
    }

    private void ConnectionStateChange(object sender, StateChangeEventArgs e)
    {
      SqlConnection sqlConnection = (SqlConnection) sender;
      if (sqlConnection.State == ConnectionState.Connecting || sqlConnection.State == ConnectionState.Executing || sqlConnection.State == ConnectionState.Fetching)
        return;
      this._connectionReady.Set();
    }
  }
}
