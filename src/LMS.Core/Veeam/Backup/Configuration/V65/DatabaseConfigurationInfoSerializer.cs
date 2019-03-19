using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal static class DatabaseConfigurationInfoSerializer
  {
    public static IDatabaseConfigurationInfo Deserialize(
      IRegistryConfigurationController controller,
      IDatabaseConfigurationInfo initial)
    {
      if (controller == null)
        throw new ArgumentNullException(nameof (controller));
      if (initial == null)
        throw new ArgumentNullException(nameof (initial));
      try
      {
        string serverName = controller.GetValue<string>("SqlServerName", initial.ServerName);
        string serverInstanceName = controller.GetValue<string>("SqlInstanceName", initial.ServerInstanceName);
        string serverInstancePipeName = controller.GetValue<string>("SqlInstancePipeName", initial.ServerInstancePipeName);
        int serverInstancePort = controller.GetValue("SqlPort", initial.ServerInstancePort);
        string initialCatalog = controller.GetValue<string>("SqlDatabaseName", initial.InitialCatalog);
        string customConnectionString = controller.GetValue<string>("SqlCustomConnectionString", initial.CustomConnectionString);
        int connectionRetryCount = controller.GetValue("ConnectRetryCount", initial.ConnectionRetryCount);
        int connectionRetryTimeout = controller.GetValue("MaxTimeoutBetweenConnectRetries", initial.ConnectionRetryTimeout);
        int connectionTimeout = controller.GetValue("SqlConnectTimeout", initial.ConnectionTimeout);
        int setupConnectionTimeout = controller.GetValue("SqlSetupConnectTimeout", initial.SetupConnectionTimeout);
        int statementTimeout = controller.GetValue("SqlStatementTimeout", initial.StatementTimeout);
        int setupStatementTimeout = controller.GetValue("SqlSetupStatementTimeout", initial.SetupStatementTimeout);
        string login = controller.GetValue<string>("SqlLogin", initial.Login);
        string str1 = controller.GetValue<string>("SqlPassword", initial.Password);
        string str2 = controller.GetValue("SqlSecuredPassword", initial.Password, RegistryControllerValueOption.DecryptString);
        string userName = controller.GetValue<string>("SqlExecuteAsUser", initial.ExecuteAsUser);
        bool enableLog = controller.GetValue("SqlEnableLog", initial.EnableLog);
        bool enableExtendedLog = controller.GetValue("SqlExtendedLog", initial.EnableExtendedLog);
        string lockInfo = controller.GetValue<string>("SqlLockInfo", initial.LockInfo);
        int maxPoolSize = controller.GetValue("SqlMaxConnectionPoolSize", initial.MaxPoolSize);
        string password = string.IsNullOrEmpty(str2) ? str1 : str2;
        return (IDatabaseConfigurationInfo) new DatabaseConfigurationInfo(serverName, serverInstanceName, serverInstancePipeName, serverInstancePort, initialCatalog, customConnectionString, connectionRetryCount, connectionRetryTimeout, connectionTimeout, setupConnectionTimeout, statementTimeout, setupStatementTimeout, login, password, userName, enableLog, enableExtendedLog, lockInfo, maxPoolSize);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to read database connection parameters.", ex);
      }
    }

    public static void Serialize(
      IRegistryConfigurationController controller,
      IDatabaseConfigurationInfo info,
      IDatabaseConfigurationInfo initial,
      bool secured)
    {
      if (controller == null)
        throw new ArgumentNullException(nameof (controller));
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      if (initial == null)
        throw new ArgumentNullException(nameof (initial));
      try
      {
        controller.SetValue("SqlServerName", info.ServerName, initial.ServerName, RegistryControllerValueOption.String);
        controller.SetValue("SqlInstanceName", info.ServerInstanceName, initial.ServerInstanceName, RegistryControllerValueOption.String);
        controller.SetValue("SqlInstancePipeName", info.ServerInstancePipeName, initial.ServerInstancePipeName, RegistryControllerValueOption.String);
        controller.SetValue("SqlPort", info.ServerInstancePort, initial.ServerInstancePort);
        controller.SetValue("SqlDatabaseName", info.InitialCatalog, initial.InitialCatalog, RegistryControllerValueOption.String);
        controller.SetValue("SqlCustomConnectionString", info.CustomConnectionString, initial.CustomConnectionString, RegistryControllerValueOption.String);
        controller.SetValue("SqlConnectTimeout", info.ConnectionTimeout, initial.ConnectionTimeout);
        controller.SetValue("SqlSetupConnectTimeout", info.SetupConnectionTimeout, initial.SetupConnectionTimeout);
        controller.SetValue("SqlStatementTimeout", info.StatementTimeout, initial.StatementTimeout);
        controller.SetValue("SqlSetupStatementTimeout", info.SetupStatementTimeout, initial.SetupStatementTimeout);
        controller.SetValue("SqlLogin", info.Login, initial.Login, RegistryControllerValueOption.String);
        controller.SetValue("SqlExecuteAsUser", info.ExecuteAsUser, initial.ExecuteAsUser, RegistryControllerValueOption.String);
        controller.SetValue("SqlEnableLog", info.EnableLog, initial.EnableLog);
        controller.SetValue("SqlExtendedLog", info.EnableExtendedLog, initial.EnableExtendedLog);
        controller.SetValue("SqlLockInfo", info.LockInfo, initial.LockInfo, RegistryControllerValueOption.String);
        if (secured)
          controller.SetValue("SqlSecuredPassword", info.Password, initial.Password, RegistryControllerValueOption.CryptString);
        else
          controller.SetValue("SqlPassword", info.Password, initial.Password, RegistryControllerValueOption.String);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to save database connection parameters.", ex);
      }
    }

    private static class Registry
    {
      public const string ServerName = "SqlServerName";
      public const string ServerInstanceName = "SqlInstanceName";
      public const string ServerInstancePipeName = "SqlInstancePipeName";
      public const string ServerInstancePort = "SqlPort";
      public const string InitialCatalog = "SqlDatabaseName";
      public const string CustomConnectionString = "SqlCustomConnectionString";
      public const string ConnectionRetryCount = "ConnectRetryCount";
      public const string ConnectionRetryTimeout = "MaxTimeoutBetweenConnectRetries";
      public const string ConnectionTimeout = "SqlConnectTimeout";
      public const string SetupConnectionTimeout = "SqlSetupConnectTimeout";
      public const string StatementTimeout = "SqlStatementTimeout";
      public const string SetupStatementTimeout = "SqlSetupStatementTimeout";
      public const string Login = "SqlLogin";
      public const string Password = "SqlPassword";
      public const string SecuredPassword = "SqlSecuredPassword";
      public const string ExecuteAsUser = "SqlExecuteAsUser";
      public const string EnableLog = "SqlEnableLog";
      public const string EnableExtendedLog = "SqlExtendedLog";
      public const string LockInfo = "SqlLockInfo";
      public const string MaxPoolSize = "SqlMaxConnectionPoolSize";
    }
  }
}
