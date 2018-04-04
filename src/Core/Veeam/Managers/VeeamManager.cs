namespace LMS.Veeam.Managers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Abp;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Abp.Timing;
    using Backup.Common;
    using Common.Constants;
    using Common.Extensions;
    using Common.Helpers;
    using Configuration;
    using Core.Common.Extensions;
    using DBManager;
    using Enums;
    using global::Hangfire.Server;
    using Mappings;
    using Microsoft.Win32;
    using Models;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamManager : DomainService, IVeeamManager
    {
        private static readonly IPerVmStoredProceduresMapping PerVmTrialQueriesMapping = new CPerVmTrialStoredProceduresMapping();
        private static readonly IPerVmStoredProceduresMapping PerVmQueriesMapping = new CPerVmStoredProceduresMapping();
        private static readonly ISqlFieldDescriptor<Guid> ObjectIdField = SqlFieldDescriptor.UniqueIdentifier("object_id");
        private static readonly ISqlFieldDescriptor<int> PlatformField = SqlFieldDescriptor.Int("platform");
        private static readonly ISqlFieldDescriptor<DateTime?> FirstStartTimeField = SqlFieldDescriptor.DateTimeNullable("first_start_time");
        private static readonly ISqlFieldDescriptor<DateTime?> LastStartTimeField = SqlFieldDescriptor.DateTimeNullable("last_start_time");

        public const string VeeamFilePath = @"C:\Program Files\Veeam\Backup and Replication\Backup\Veeam.Backup.Service.exe";
        public const string Veeam90FilePath = @"C:\Program Files\Veeam\Backup and Replication\Veeam.Backup.Service.exe";

        private VmLicensingInfo FromReader(IDataReader reader)
        {
            return new VmLicensingInfo(ObjectIdField.Read(reader), FirstStartTimeField.Read(reader), LastStartTimeField.Read(reader), (EPlatform)PlatformField.Read(reader), reader.GetClass<string>("host_name"), string.Empty, reader.GetClass<string>("object_name"));
        }

        public List<VmLicensingInfo> GetAllVmInfos(PerformContext performContext, EPlatform platform)
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
                Logger.Error(performContext,ex.Message);
                Logger.Debug(performContext,"Exception",ex);
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

        public int GetProtectedVmCount(PerformContext performContext)
        {
            try
            {
                var localDbAccessor = new LocalDbAccessor(GetConnectionString());

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
                Logger.Error(performContext,ex.Message);
                Logger.Debug(performContext,ex.Message, ex);
                return 0;
            }

            return 0;
        }

        public int GetProtectedVmsCount(PerformContext performContext, EPlatform platform)
        {
            try
            {
                var localDbAccessor = new LocalDbAccessor(GetConnectionString());

                using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmsCount", DbAccessor.MakeParam("@platform", (int)platform)).CreateDataReader())
                {
                    if (dataReader.Read())
                    {
                        return (int)dataReader["protected_vms_count"];
                    }
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Error(performContext,ex.Message);
                Logger.Debug(performContext,ex.Message, ex);
                return 0;
            }

            return 0;
        }

        public VmsCounterInfo GetVmsCounters(PerformContext performContext, EPlatform platform, bool useTrialStrategy)
        {
            try
            {
                var localDbAccessor = new LocalDbAccessor(GetConnectionString());
                using (DataTableReader dataReader = localDbAccessor.GetDataTable(GetMapping(useTrialStrategy).GetVmsNumbers, DbAccessor.MakeParam("@platform", (int)platform)).CreateDataReader())
                {
                    dataReader.Read();
                    return new VmsCounterInfo(dataReader.GetValue<int>("vm_active"), dataReader.GetValue<int>("vm_trial"));
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Error(performContext,ex.Message);
                Logger.Debug(performContext,ex.Message, ex);
                return new VmsCounterInfo(0, 0);
            }
        }

        public bool IsInstalled(PerformContext performContext)
        {
            try
            {
                return CommonHelpers.IsApplictionInstalled(Constants.VeeamApplicationName);
            }
            catch (Exception ex)
            {
                Logger.Error(performContext,ex.Message);
                Logger.Debug(performContext,ex.Message, ex);
                return false;
            }
        }

        public bool IsOnline()
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

        public string GetVersion()
        {
            if (File.Exists(VeeamFilePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(VeeamFilePath);
                SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, version.FileVersion);
                return version.FileVersion;
            }

            if (File.Exists(Veeam90FilePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(Veeam90FilePath);
                SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, version.FileVersion);
                return version.FileVersion;
            }

            SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, string.Empty);
            return null;
        }


        public Veeam GetLicensingInformation(PerformContext performContext)
        {
            Veeam veeam = new Veeam();

            try
            {
                veeam.LicenseType = VeeamLicense.TypeEx;
            }
            catch (Exception ex)
            {
                Logger.Error(performContext,"There was an error while getting the license information from the registry. We'll therefore assume its an evaluation license.");
                Logger.Debug(performContext,ex.Message, ex);

                veeam.LicenseType = LicenseTypeEx.Evaluation;
            }

            veeam.ProgramVersion = GetVersion();

            Version programVersion = Version.Parse(veeam.ProgramVersion);

            var (vsphere, hyperv) = GetVirtualMachineCount(performContext, programVersion, veeam.LicenseType);
            veeam.vSphere = vsphere;
            veeam.HyperV = hyperv;

            veeam.CheckInTime = new DateTimeOffset(Clock.Now);
            veeam.ClientVersion = SettingManagerHelper.ClientVersion;
            veeam.Edition = VeeamLicense.Edition;
            veeam.ExpirationDate = VeeamLicense.ExpirationDate;
            veeam.Id = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>(); // auth service
            veeam.SupportId = VeeamLicense.SupportId;
            veeam.TenantId = SettingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId); // auth service


            return veeam;
        }
        protected (int vsphere, int hyperv) GetVirtualMachineCount(PerformContext performContext, Version programVersion, LicenseTypeEx licenseType)
        {
            if (programVersion.Major == 9 && programVersion.Minor == 5)
            {
                if (programVersion.Revision == 1038)
                {
                    return GetVirtualMachineCount9501038(performContext);
                }

                return GetVirtualMachineCount95(performContext, licenseType);
            }

            // default
            return GetVirtualMachineCount90(performContext);
        }

        protected (int vsphere, int hyperv) GetVirtualMachineCount9501038(PerformContext performContext)
        {
            int vsphere = GetProtectedVmsCount(performContext, EPlatform.EVmware);
            int hyperv = GetProtectedVmsCount(performContext, EPlatform.EHyperV);

            return (vsphere, hyperv);
        }

        protected (int vsphere, int hyperv) GetVirtualMachineCount95(PerformContext performContext, LicenseTypeEx licenseType)
        {
            bool evaluation = licenseType == LicenseTypeEx.Evaluation;

            VmsCounterInfo vSphereCounterInfo = GetVmsCounters(performContext, EPlatform.EVmware, evaluation);
            VmsCounterInfo hypervCounterInfo = GetVmsCounters(performContext, EPlatform.EHyperV, evaluation);

            int vsphere = evaluation ? vSphereCounterInfo.TrialVmsCount : vSphereCounterInfo.NonTrialVmsCount;
            int hyperv = evaluation ? hypervCounterInfo.TrialVmsCount : hypervCounterInfo.NonTrialVmsCount;

            return (vsphere, hyperv);
        }

        protected (int vsphere, int hyperv) GetVirtualMachineCount90(PerformContext performContext)
        {
            int vsphere = GetAllVmInfos(performContext, EPlatform.EVmware).Count(item => item.State == EVmLicensingStatus.Managed);
            int hyperv = GetAllVmInfos(performContext, EPlatform.EHyperV).Count(item => item.State == EVmLicensingStatus.Managed);

            return (vsphere, hyperv);
        }
    }
}