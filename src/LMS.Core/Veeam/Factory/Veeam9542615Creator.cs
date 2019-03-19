namespace LMS.Core.Veeam.Factory
{
    using System;
    using Backup.Common;
    using Backup.Core;
    using FluentResults;
    using Helpers;
    using Models;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using Serilog;
    using Services.Authentication;
    using ILogger = Serilog.ILogger;

    public class Veeam9542615Creator : VeeamCreator
    {
        private readonly ILogger _logger = Log.ForContext<Veeam9542615Creator>();

        public Veeam9542615Creator(Version applicationVersion)
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
            payload.vSphere = SInstancesLicenseCountersHelper.GetProtectedVmsCount(CLicensePlatform.Vmware);
            payload.HyperV = 0;
            payload.ClientVersion = SettingManagerHelper.ClientVersion;
            payload.Edition = VeeamLicense.Edition;
            payload.ExpirationDate = VeeamLicense.ExpirationDate;
            payload.Hostname = Environment.MachineName;
            payload.Id = PortalAuthenticationService.Instance.GetDevice();
            payload.TenantId = Convert.ToInt32(PortalAuthenticationService.Instance.GetAccount());

            return Results.Ok(payload);
        }
    }
}