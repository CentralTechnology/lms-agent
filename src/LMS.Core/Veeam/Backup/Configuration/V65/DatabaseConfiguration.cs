using Microsoft.Win32;
using System;
using System.Text;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public class DatabaseConfiguration : IDatabaseConfiguration, IConfiguration
  {
    private readonly string _registryKey;

    public IDatabaseConfigurationInfo Info { get; private set; }

    public IDatabaseConfigurationInfo Default { get; private set; }

    public RegistryView RegistryView { get; private set; }

    public DatabaseConfiguration(
      string registryKey,
      IDatabaseConfigurationInfo info,
      IDatabaseConfigurationInfo defaultInfo,
      RegistryView registryView = RegistryView.Default)
    {
      if (string.IsNullOrEmpty(registryKey))
        throw new ArgumentNullException(nameof (registryKey));
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      if (defaultInfo == null)
        throw new ArgumentNullException(nameof (defaultInfo));
      this._registryKey = registryKey;
      this.RegistryView = registryView;
      this.Info = info;
      this.Default = defaultInfo;
    }

    public string ConnectionString
    {
      get
      {
        return (string) new ConnectionStringBuilder(this.Info, this.Default);
      }
    }

    public IDatabaseConfiguration Copy()
    {
      return (IDatabaseConfiguration) new DatabaseConfiguration(this._registryKey, this.Info.Copy(), this.Default, RegistryView.Default);
    }

    public void Load(RegistryView registryView = RegistryView.Default)
    {
      using (IRegistryConfigurationController controller = RegistryConfigurationController.Create(this._registryKey, false, registryView))
        this.Info = DatabaseConfigurationInfoSerializer.Deserialize(controller, this.Default);
    }

    public void Save(bool secured, RegistryView registryView = RegistryView.Default)
    {
      using (IRegistryConfigurationController controller = RegistryConfigurationController.Create(this._registryKey, true, registryView))
        DatabaseConfigurationInfoSerializer.Serialize(controller, this.Info, this.Default, secured);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("Server: [{0}], ", (object) this.Info.ServerName);
      stringBuilder.AppendFormat("ServerInstanceName: [{0}], ", (object) this.Info.ServerInstanceName);
      stringBuilder.AppendFormat("ServerInstancePipeName: [{0}], ", (object) this.Info.ServerInstancePipeName);
      stringBuilder.AppendFormat("ServerInstancePort: [{0}], ", (object) this.Info.ServerInstancePort);
      stringBuilder.AppendFormat("InitialCatalog: [{0}], ", (object) this.Info.InitialCatalog);
      stringBuilder.AppendFormat("SqlAuthentication: [{0}], ", (object) this.Info.SqlAuthentication);
      stringBuilder.AppendFormat("User: [{0}], ", this.Info.SqlAuthentication ? (object) this.Info.Login : (object) "current");
      stringBuilder.AppendFormat("Password: [{0}], ", this.Info.SqlAuthentication ? (object) "*****" : (object) string.Empty);
      stringBuilder.AppendFormat("ImpersonateUser: [{0}], ", (object) this.Info.ImpersonateUser);
      stringBuilder.AppendFormat("UserName: [{0}], ", (object) this.Info.ExecuteAsUser);
      stringBuilder.AppendFormat("ConnectionRetryCount: [{0}], ", (object) this.Info.ConnectionRetryCount);
      stringBuilder.AppendFormat("ConnectionRetryTimeout: [{0}], ", (object) this.Info.ConnectionRetryTimeout);
      stringBuilder.AppendFormat("ConnectionTimeout: [{0}], ", (object) this.Info.ConnectionTimeout);
      stringBuilder.AppendFormat("SetupConnectionTimeout: [{0}], ", (object) this.Info.SetupConnectionTimeout);
      stringBuilder.AppendFormat("StatementTimeout: [{0}], ", (object) this.Info.StatementTimeout);
      stringBuilder.AppendFormat("SetupStatementTimeout: [{0}], ", (object) this.Info.SetupStatementTimeout);
      stringBuilder.AppendFormat("EnableLog: [{0}], ", (object) this.Info.EnableLog);
      stringBuilder.AppendFormat("EnableExtendedLog: [{0}], ", (object) this.Info.EnableExtendedLog);
      return stringBuilder.ToString();
    }

    public static IDatabaseConfiguration Create(
      string registryKey,
      IDatabaseConfigurationInfo initial,
      RegistryView registryView = RegistryView.Default)
    {
      return (IDatabaseConfiguration) new DatabaseConfiguration(registryKey, initial.Copy(), initial, registryView);
    }

    public static IDatabaseConfiguration Load(
      string registryKey,
      IDatabaseConfigurationInfo initial,
      RegistryView registryView = RegistryView.Default)
    {
      IDatabaseConfiguration databaseConfiguration = DatabaseConfiguration.Create(registryKey, initial, registryView);
      databaseConfiguration.Load(registryView);
      return databaseConfiguration;
    }
  }
}
