namespace LMS.Core.Veeam.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Abp;
    using Backup.Common;
    using Backup.DBManager;
    using DBManager;
    using Enums;
    using Extensions;
    using FluentResults;
    using Helpers;
    using Microsoft.Win32;
    using Models;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using Serilog;
    using Services.Authentication;
    using ILogger = Serilog.ILogger;
    using LocalDbAccessor = DBManager.LocalDbAccessor;

    internal class Veeam90Creator : VeeamCreator
    {
        private static readonly ISqlFieldDescriptor<DateTime?> FirstStartTimeField = SqlFieldDescriptor.DateTimeNullable("first_start_time");
        private static readonly ISqlFieldDescriptor<DateTime?> LastStartTimeField = SqlFieldDescriptor.DateTimeNullable("last_start_time");
        private static readonly ISqlFieldDescriptor<Guid> ObjectIdField = SqlFieldDescriptor.UniqueIdentifier("object_id");
        private static readonly ISqlFieldDescriptor<int> PlatformField = SqlFieldDescriptor.Int("platform");
        private readonly ILogger _logger = Log.ForContext<Veeam90Creator>();

        public Veeam90Creator(Version applicationVersion)
            : base(applicationVersion)
        {
        }

        public override Result<Veeam> Create()
        {
            var payload = new Veeam();
            var result = new Result<Veeam>();

            try
            {
                payload.LicenseType = VeeamLicense.TypeEx;
            }
            catch (Exception ex)
            {
                const string msg = "There was an error while getting the license information from the registry. We'll therefore assume its an evaluation license.";
                result.WithError(msg);
                _logger.Error(ex.Message, msg);

                payload.LicenseType = LicenseTypeEx.Evaluation;
            }

            payload.ProgramVersion = ApplicationVersion.ToString();
            payload.vSphere = GetAllVmInfos(EPlatform.EVmware).Count(item => item.State == EVmLicensingStatus.Managed);
            payload.HyperV = GetAllVmInfos(EPlatform.EHyperV).Count(item => item.State == EVmLicensingStatus.Managed);
            payload.ClientVersion = SettingManagerHelper.ClientVersion;
            payload.Edition = VeeamLicense.Edition;
            payload.ExpirationDate = VeeamLicense.ExpirationDate;
            payload.Hostname = Environment.MachineName;
            payload.Id = PortalAuthenticationService.Instance.GetDevice();
            payload.TenantId = Convert.ToInt32(PortalAuthenticationService.Instance.GetAccount());

            return Results.Ok(payload);
        }

        private static VmLicensingInfo FromReader(IDataReader reader)
        {
            return new VmLicensingInfo(ObjectIdField.Read(reader), FirstStartTimeField.Read(reader), LastStartTimeField.Read(reader), (EPlatform) PlatformField.Read(reader), reader.GetClass<string>("host_name"), string.Empty, reader.GetClass<string>("object_name"));
        }

        private static IEnumerable<VmLicensingInfo> GetAllVmInfos(EPlatform platform)
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

        private static string GetConnectionString()
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
    }
}