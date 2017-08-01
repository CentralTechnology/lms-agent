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

    public class VeeamManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            return true;
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
            return "9.5.0.1038";
            string veeamVersion = Constants.VeeamApplicationName.GetApplicationVersion().ToString();
            if (veeamVersion == null)
            {
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, string.Empty);
                return null;
            }

            SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, veeamVersion);
            return veeamVersion;
        }

        public int GetProtectedVms()
        {
            var localDbAccessor = new LocalDbAccessor(VeeamFactory.VeeamManager().GetConnectionString());

            using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmCount", DbAccessor.MakeParam("@days", Constants.VeeamProtectedVmCountDays)).CreateDataReader())
            {
                if (dataReader.Read())
                {
                    return (int)dataReader["vm_count"];
                }
            }

            return 0;
        }

        public Veeam Build(LicenseManager licenseManager)
        {
            Veeam veeam = new Veeam();

            veeam.Edition = licenseManager.GetPropertyNoThrow<LicenseEditions>("Edition");
            veeam.ExpirationDate = licenseManager.GetPropertyNoThrow<DateTime>("Expiration date");
            

            return veeam;
        }
    }
}