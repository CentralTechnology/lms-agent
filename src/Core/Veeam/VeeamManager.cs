namespace Core.Veeam
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using Abp;
    using Administration;
    using Common.Constants;
    using Common.Extensions;
    using Factory;
    using Microsoft.Win32;
    using NLog;

    public class VeeamManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [SuppressMessage("ReSharper", "JoinNullCheckWithUsage")]
        public string GetConnectionString()
        {
            string sqlDatabaseName = null;
            string sqlInstanceName = null;
            string sqlServerName = null;

            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Veeam");
            if (key == null)
            {
                throw new AbpException("Unable to locate the Veeam registry hive. Please make sure Veeam is installed correctly.");
            }

            foreach (string keyName in key.GetSubKeyNames())
            {
                if (keyName == "Veeam Backup and Replication")
                {
                    RegistryKey subKey = key.OpenSubKey(keyName);
                    if (subKey == null)
                    {
                        throw new AbpException("Unable to locate the Veeam registry hive. Please make sure Veeam is installed correctly.");
                    }

                    sqlDatabaseName = subKey.GetValue("SqlDatabaseName") as string;
                    sqlInstanceName = subKey.GetValue("SqlInstanceName") as string;
                    sqlServerName = subKey.GetValue("SqlServerName") as string;
                }
            }

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

        public Version VeeamVersion()
        {
            Version veeamVersion = Constants.VeeamApplicationName.GetApplicationVersion();
            if (veeamVersion == null)
            {
                SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, string.Empty);
                return null;
            }

            SettingFactory.SettingsManager().ChangeSetting(SettingNames.VeeamVersion, veeamVersion.ToString());
            return veeamVersion;
        }
    }
}