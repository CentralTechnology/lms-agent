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
    using Backup.Common;
    using Common.Extensions;
    using Common.Helpers;
    using Core.Administration;
    using Core.Common.Constants;
    using Core.Configuration;
    using DBManager;
    using Enums;
    using Mappings;
    using Microsoft.Win32;
    using Models;
    using NLog;
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
                Logger.Debug("Exception",ex);
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
                        return (int)dataReader["vm_count"];
                    }
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug("Exception",ex);
                return 0;
            }

            return 0;
        }

        public int GetProtectedVmsCount(EPlatform platform)
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
                Logger.Error(ex.Message);
                Logger.Debug("Exception", ex);
                return 0;
            }

            return 0;
        }

        public VmsCounterInfo GetVmsCounters(EPlatform platform, bool useTrialStrategy)
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
                Logger.Error(ex.Message);
                Logger.Debug("Exception", ex);
                return new VmsCounterInfo(0, 0);
            }
        }

        public bool IsInstalled()
        {
            try
            {
                return CommonExtensions.IsApplictionInstalled(Constants.VeeamApplicationName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug("Exception", ex);
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


        public Veeam GetLicensingInformation(Veeam veeam)
        {
            try
            {
                veeam.LicenseType = VeeamLicense.TypeEx;
            }
            catch (Exception ex)
            {
                Logger.Error("There was an error while getting the license information from the registry. We'll therefore assume its an evaluation license.");
                Logger.Debug("Exception",ex);
                veeam.LicenseType = LicenseTypeEx.Evaluation;
            }

            veeam.ProgramVersion = GetVersion();

            Version programVersion = Version.Parse(veeam.ProgramVersion);

            var virtualMachines = GetVirtualMachineCount(programVersion, veeam.LicenseType);
            veeam.vSphere = virtualMachines.vsphere;
            veeam.HyperV = virtualMachines.hyperv;

            veeam.ClientVersion = SettingManagerHelper.Instance.ClientVersion;
            veeam.Edition = VeeamLicense.Edition;
            veeam.ExpirationDate = VeeamLicense.ExpirationDate;
            veeam.Id = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();
            veeam.SupportId = VeeamLicense.SupportId;
            veeam.TenantId = SettingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);

            return veeam;
        }
        protected (int vsphere, int hyperv) GetVirtualMachineCount(Version programVersion, LicenseTypeEx licenseType)
        {
            if (programVersion.Major == 9 && programVersion.Minor == 5)
            {
                if (programVersion.Revision == 1038)
                {
                    return GetVirtualMachineCount9501038();
                }

                return GetVirtualMachineCount95(licenseType);
            }

            // default
            return GetVirtualMachineCount90();
        }

        protected (int vsphere, int hyperv) GetVirtualMachineCount9501038()
        {
            int vsphere = GetProtectedVmsCount(EPlatform.EVmware);
            int hyperv = GetProtectedVmsCount(EPlatform.EHyperV);

            return (vsphere, hyperv);
        }

        protected (int vsphere, int hyperv) GetVirtualMachineCount95(LicenseTypeEx licenseType)
        {
            bool evaluation = licenseType == LicenseTypeEx.Evaluation;

            VmsCounterInfo vSphereCounterInfo = GetVmsCounters(EPlatform.EVmware, evaluation);
            VmsCounterInfo hypervCounterInfo = GetVmsCounters(EPlatform.EHyperV, evaluation);

            int vsphere = evaluation ? vSphereCounterInfo.TrialVmsCount : vSphereCounterInfo.NonTrialVmsCount;
            int hyperv = evaluation ? hypervCounterInfo.TrialVmsCount : hypervCounterInfo.NonTrialVmsCount;

            return (vsphere, hyperv);
        }

        protected (int vsphere, int hyperv) GetVirtualMachineCount90()
        {
            int vsphere = GetAllVmInfos(EPlatform.EVmware).Count(item => item.State == EVmLicensingStatus.Managed);
            int hyperv = GetAllVmInfos(EPlatform.EHyperV).Count(item => item.State == EVmLicensingStatus.Managed);

            return (vsphere, hyperv);
        }
    }
}