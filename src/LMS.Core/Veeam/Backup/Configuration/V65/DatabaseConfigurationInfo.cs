using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public class DatabaseConfigurationInfo : IDatabaseConfigurationInfo
  {
    public string ServerName { get; set; }

    public string ServerInstanceName { get; set; }

    public string ServerInstancePipeName { get; set; }

    public int ServerInstancePort { get; set; }

    public string InitialCatalog { get; set; }

    public bool MultipleActiveResultSets { get; set; }

    public string CustomConnectionString { get; set; }

    public int ConnectionRetryCount { get; set; }

    public int ConnectionRetryTimeout { get; set; }

    public int ConnectionTimeout { get; set; }

    public int SetupConnectionTimeout { get; set; }

    public int StatementTimeout { get; set; }

    public int SetupStatementTimeout { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public string ExecuteAsUser { get; set; }

    public bool EnableLog { get; set; }

    public bool EnableExtendedLog { get; set; }

    public string LockInfo { get; set; }

    public int MaxPoolSize { get; set; }

    public DatabaseConfigurationInfo(
      string serverName,
      string serverInstanceName,
      string serverInstancePipeName,
      int serverInstancePort,
      string initialCatalog,
      string customConnectionString,
      int connectionRetryCount,
      int connectionRetryTimeout,
      int connectionTimeout,
      int setupConnectionTimeout,
      int statementTimeout,
      int setupStatementTimeout,
      string login,
      string password,
      string userName,
      bool enableLog,
      bool enableExtendedLog,
      string lockInfo,
      int maxPoolSize)
    {
      if (string.IsNullOrEmpty(serverName))
        throw new ArgumentNullException(nameof (serverName));
      if (string.IsNullOrEmpty(initialCatalog))
        throw new ArgumentNullException(nameof (initialCatalog));
      this.ServerName = serverName;
      this.ServerInstanceName = serverInstanceName ?? string.Empty;
      this.ServerInstancePipeName = serverInstancePipeName ?? string.Empty;
      this.ServerInstancePort = serverInstancePort;
      this.InitialCatalog = initialCatalog;
      this.CustomConnectionString = customConnectionString ?? string.Empty;
      this.ConnectionRetryCount = connectionRetryCount;
      this.ConnectionRetryTimeout = connectionRetryTimeout;
      this.ConnectionTimeout = connectionTimeout;
      this.SetupConnectionTimeout = setupConnectionTimeout;
      this.StatementTimeout = statementTimeout;
      this.SetupStatementTimeout = setupStatementTimeout;
      this.Login = login ?? string.Empty;
      this.Password = password ?? string.Empty;
      this.ExecuteAsUser = userName ?? string.Empty;
      this.EnableLog = enableLog;
      this.EnableExtendedLog = enableExtendedLog;
      this.LockInfo = lockInfo ?? string.Empty;
      this.MaxPoolSize = maxPoolSize;
    }

    public string ServerInstanceFullName
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.ServerInstancePipeName))
          return (string) new ProductServerName(this.ServerName, this.ServerInstanceName);
        return this.ServerInstancePipeName;
      }
    }

    public bool SqlAuthentication
    {
      get
      {
        return !string.IsNullOrEmpty(this.Login);
      }
    }

    public bool ImpersonateUser
    {
      get
      {
        return !string.IsNullOrEmpty(this.ExecuteAsUser);
      }
    }

    public IDatabaseConfigurationInfo Copy()
    {
      return (IDatabaseConfigurationInfo) this.MemberwiseClone();
    }
  }
}
