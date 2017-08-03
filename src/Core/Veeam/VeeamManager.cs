namespace Core.Veeam
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using Abp;
    using Administration;
    using Common.Constants;
    using Common.Extensions;
    using DBManager;
    using Factory;
    using Microsoft.Win32;
    using NLog;
    using Abp.Extensions;

    public class VeeamManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IPerVmStoredProceduresMapping PerVmTrialQueriesMapping = new CPerVmTrialStoredProceduresMapping();
        private static readonly IPerVmStoredProceduresMapping PerVmQueriesMapping = new CPerVmStoredProceduresMapping();

        [SuppressMessage("ReSharper", "JoinNullCheckWithUsage")]
        public string GetConnectionString()
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Veeam");
            if (key == null)
            {
                throw new AbpException("Unable to locate the Veeam registry hive. Please make sure Veeam is installed correctly.");
            }

            string sqlDatabaseName = key.GetSearchValue<string>("Veeam Backup and Replication", "SqlDatabaseName");
            string sqlInstanceName = key.GetSearchValue<string>("Veeam Backup and Replication", "SqlInstanceName");
            string sqlServerName = key.GetSearchValue<string>("Veeam Backup and Replication", "SqlServerName");

            if (sqlDatabaseName == null || sqlInstanceName == null || sqlServerName == null)
            {
                throw new AbpException("Unable to determine the correct connection string for the Veeam database.");
            }

            var connectionString = new SqlConnectionStringBuilder
            {
                InitialCatalog = sqlDatabaseName,
                IntegratedSecurity = true,
                DataSource = $"{sqlServerName}\\{sqlInstanceName}"
            };

            return connectionString.ConnectionString;
        }

        private static IPerVmStoredProceduresMapping GetMapping(bool useTrialStrategy)
        {
            return !useTrialStrategy ? PerVmQueriesMapping : PerVmTrialQueriesMapping;
        }

        public int GetProtectedVmCount()
        {
            var localDbAccessor = new LocalDbAccessor(VeeamFactory.VeeamManager().GetConnectionString());

            using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmCount", DbAccessor.MakeParam("@days", Constants.VeeamProtectedVmCountDays)).CreateDataReader())
            {
                if (dataReader.Read())
                {
                    return (int) dataReader["vm_count"];
                }
            }

            return 0;
        }

        public VmsCounterInfo GetVmsCounters(EPlatform platform, bool useTrialStrategy)
        {
            var localDbAccessor = new LocalDbAccessor(VeeamFactory.VeeamManager().GetConnectionString());
            using (DataTableReader dataReader = localDbAccessor.GetDataTable(GetMapping(useTrialStrategy).GetVmsNumbers, DbAccessor.MakeParam("@platform", (int) platform)).CreateDataReader())
            {
                dataReader.Read();
                return new VmsCounterInfo(dataReader.GetValue<int>("vm_active"), dataReader.GetValue<int>("vm_trial"));
            }
        }

        public bool VeeamInstalled()
        {
            try
            {
                return Constants.VeeamApplicationName.IsApplictionInstalled();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                return false;
            }
        }

        public bool VeeamOnline()
        {
            IPAddress localhost = IPAddress.Parse("127.0.0.1");

            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(localhost, 9392);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error("Unable to contact the local Veeam server. Please make sure the services are started.");
                    Logger.Debug(ex);
                    return false;
                }
            }
        }

        public string VeeamVersion()
        {
            string veeamVersion = Constants.VeeamApplicationName.GetApplicationVersion().ToString();
            if (veeamVersion.IsNullOrEmpty())
            {
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, string.Empty);
                return null;
            }

            SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, veeamVersion);
            return veeamVersion;
        }
    }
}