namespace LMS.Core.Veeam.Factory
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Abp;
    using Backup.Common;
    using DBManager;
    using Extensions;
    using FluentResults;
    using Helpers;
    using Microsoft.Win32;
    using Models;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using Serilog;
    using Services.Authentication;
    using ILogger = Serilog.ILogger;

    public class Veeam9501038Creator : VeeamCreator
    {
        private readonly ILogger _logger = Log.ForContext<Veeam9501038Creator>();

        public Veeam9501038Creator(Version applicationVersion)
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
            payload.vSphere = GetProtectedVmsCount(EPlatform.EVmware);
            payload.HyperV = GetProtectedVmsCount(EPlatform.EHyperV);
            payload.ClientVersion = SettingManagerHelper.ClientVersion;
            payload.Edition = VeeamLicense.Edition;
            payload.ExpirationDate = VeeamLicense.ExpirationDate;
            payload.Hostname = Environment.MachineName;
            payload.Id = PortalAuthenticationService.Instance.GetDevice();
            payload.TenantId = Convert.ToInt32(PortalAuthenticationService.Instance.GetAccount());

            return Results.Ok(payload);
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

        private static int GetProtectedVmsCount(EPlatform platform)
        {
            var localDbAccessor = new LocalDbAccessor(GetConnectionString());

            using (DataTableReader dataReader = localDbAccessor.GetDataTable("GetProtectedVmsCount", DbAccessor.MakeParam("@platform", (int) platform)).CreateDataReader())
            {
                if (dataReader.Read())
                {
                    return (int) dataReader["protected_vms_count"];
                }
            }

            return 0;
        }
    }
}