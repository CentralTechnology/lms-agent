namespace Core.Veeam
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using Abp;
    using Abp.Extensions;
    using Administration;
    using Backup.Common;
    using Common.Constants;
    using Common.Extensions;
    using DBManager;
    using Factory;
    using Microsoft.Win32;
    using NLog;

    public class VeeamManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IPerVmStoredProceduresMapping PerVmTrialQueriesMapping = new CPerVmTrialStoredProceduresMapping();
        private static readonly IPerVmStoredProceduresMapping PerVmQueriesMapping = new CPerVmStoredProceduresMapping();
        private static readonly ISqlFieldDescriptor<Guid> ObjectIdField = SqlFieldDescriptor.UniqueIdentifier("object_id");
        private static readonly ISqlFieldDescriptor<int> PlatformField = SqlFieldDescriptor.Int("platform");
        private static readonly ISqlFieldDescriptor<DateTime?> FirstStartTimeField = SqlFieldDescriptor.DateTimeNullable("first_start_time");
        private static readonly ISqlFieldDescriptor<DateTime?> LastStartTimeField = SqlFieldDescriptor.DateTimeNullable("last_start_time");

        private VmLicensingInfo FromReader(IDataReader reader)
        {
            return new VmLicensingInfo(ObjectIdField.Read(reader), FirstStartTimeField.Read(reader), LastStartTimeField.Read(reader), (EPlatform)PlatformField.Read(reader), reader.GetClass<string>("host_name"), string.Empty, reader.GetClass<string>("object_name"));
        }

        public List<VmLicensingInfo> GetAllVmInfos(EPlatform platform)
        {
            try
            {
                var vmLicensingInfoList = new List<VmLicensingInfo>();
                using (DataTableReader dataReader = new LocalDbAccessor(GetConnectionString()).GetDataTable("[dbo].[GetVmLicensing]", DbAccessor.MakeParam("@platform", (int)platform)).CreateDataReader())
                {
                    while (dataReader.Read())
                    {
                        vmLicensingInfoList.Add(FromReader(dataReader));
                    }
                }

                return vmLicensingInfoList;
            }
            catch (Win32Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                return new List<VmLicensingInfo>();
            }
        }

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
            try
            {
                var localDbAccessor = new LocalDbAccessor(VeeamFactory.VeeamManager().GetConnectionString());

                using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmCount", DbAccessor.MakeParam("@days", Constants.VeeamProtectedVmCountDays)).CreateDataReader())
                {
                    if (dataReader.Read())
                    {
                        return (int)dataReader["vm_count"];
                    }
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                return 0;
            }

            return 0;
        }

        public VmsCounterInfo GetVmsCounters(EPlatform platform, bool useTrialStrategy)
        {
            try
            {
                var localDbAccessor = new LocalDbAccessor(VeeamFactory.VeeamManager().GetConnectionString());
                using (DataTableReader dataReader = localDbAccessor.GetDataTable(GetMapping(useTrialStrategy).GetVmsNumbers, DbAccessor.MakeParam("@platform", (int)platform)).CreateDataReader())
                {
                    dataReader.Read();
                    return new VmsCounterInfo(dataReader.GetValue<int>("vm_active"), dataReader.GetValue<int>("vm_trial"));
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                return new VmsCounterInfo(0,0);
            }
        }

        public bool VeeamInstalled()
        {
            try
            {
                return CommonExtensions.IsApplictionInstalled(Constants.VeeamApplicationName);
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
                catch (SocketException)
                {
                    return false;
                }
            }
        }

        public string VeeamVersion()
        {
            string veeamVersion = CommonExtensions.GetApplicationVersion(Constants.VeeamApplicationName).ToString();
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