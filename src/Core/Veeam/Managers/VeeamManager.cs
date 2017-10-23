﻿namespace Core.Veeam.Managers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using Abp;
    using Administration;
    using Backup.Common;
    using Common.Constants;
    using Common.Extensions;
    using DBManager;
    using Microsoft.Win32;
    using NLog;

    public class VeeamManager
    {
        private static readonly IPerVmStoredProceduresMapping PerVmTrialQueriesMapping = new CPerVmTrialStoredProceduresMapping();
        private static readonly IPerVmStoredProceduresMapping PerVmQueriesMapping = new CPerVmStoredProceduresMapping();
        private static readonly ISqlFieldDescriptor<Guid> ObjectIdField = SqlFieldDescriptor.UniqueIdentifier("object_id");
        private static readonly ISqlFieldDescriptor<int> PlatformField = SqlFieldDescriptor.Int("platform");
        private static readonly ISqlFieldDescriptor<DateTime?> FirstStartTimeField = SqlFieldDescriptor.DateTimeNullable("first_start_time");
        private static readonly ISqlFieldDescriptor<DateTime?> LastStartTimeField = SqlFieldDescriptor.DateTimeNullable("last_start_time");
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly SettingManager SettingManager = new SettingManager();

        private VmLicensingInfo FromReader(IDataReader reader)
        {
            return new VmLicensingInfo(ObjectIdField.Read(reader), FirstStartTimeField.Read(reader), LastStartTimeField.Read(reader), (EPlatform) PlatformField.Read(reader), reader.GetClass<string>("host_name"), string.Empty, reader.GetClass<string>("object_name"));
        }

        public List<VmLicensingInfo> GetAllVmInfos(EPlatform platform)
        {
            try
            {
                var vmLicensingInfoList = new List<VmLicensingInfo>();
                using (DataTableReader dataReader = new LocalDbAccessor(GetConnectionString()).GetDataTable("[dbo].[GetVmLicensing]", DbAccessor.MakeParam("@platform", (int) platform)).CreateDataReader())
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
                var localDbAccessor = new LocalDbAccessor(GetConnectionString());

                using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmCount", DbAccessor.MakeParam("@days", Constants.VeeamProtectedVmCountDays)).CreateDataReader())
                {
                    if (dataReader.Read())
                    {
                        return (int) dataReader["vm_count"];
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

        public int GetProtectedVmsCount(EPlatform platform)
        {
            try
            {
                var localDbAccessor = new LocalDbAccessor(GetConnectionString());

                using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmsCount", DbAccessor.MakeParam("@platform", (int) platform)).CreateDataReader())
                {
                    if (dataReader.Read())
                    {
                        return (int) dataReader["protected_vms_count"];
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
                var localDbAccessor = new LocalDbAccessor(GetConnectionString());
                using (DataTableReader dataReader = localDbAccessor.GetDataTable(GetMapping(useTrialStrategy).GetVmsNumbers, DbAccessor.MakeParam("@platform", (int) platform)).CreateDataReader())
                {
                    dataReader.Read();
                    return new VmsCounterInfo(dataReader.GetValue<int>("vm_active"), dataReader.GetValue<int>("vm_trial"));
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex);
                return new VmsCounterInfo(0, 0);
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
            FileVersionInfo veeamFile = null;

            try
            {
                veeamFile = FileVersionInfo.GetVersionInfo(@"C:\Program Files\Veeam\Backup and Replication\Backup\Veeam.Backup.Service.exe");
            }
            catch (FileNotFoundException ex)
            {
                Logger.Debug(ex);
            }

            if (veeamFile == null)
            {
                try
                {
                    // file path for veeam 9.0
                    veeamFile = FileVersionInfo.GetVersionInfo(@"C:\Program Files\Veeam\Backup and Replication\Veeam.Backup.Service.exe");
                }
                catch (FileNotFoundException ex)
                {
                    Logger.Error("Unable to find the Veeam.Backup.Service executable. Unable to determine the correct program version.");
                    Logger.Debug(ex);
                    SettingManager.ChangeSetting(SettingNames.VeeamVersion, string.Empty);
                    return null;
                }
            }

            SettingManager.ChangeSetting(SettingNames.VeeamVersion, veeamFile.FileVersion);
            return veeamFile.FileVersion;
        }
    }
}