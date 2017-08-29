namespace Core.Common.Extensions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp;
    using Administration;
    using NLog;
    using Veeam;
    using Veeam.Backup.Common;

    public static class VeeamExtensions
    {
        private static readonly LicenseManager LicenseManager = new LicenseManager();
        private static readonly VeeamManager VeeamManager = new VeeamManager();
        private static readonly SettingManager SettingManager = new SettingManager();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task CollectInformation(this Veeam veeam)
        {
            try
            {
                veeam.LicenseType = VeeamLicense.TypeEx;
            }
            catch (Exception ex)
            {
                Logger.Error("There was an error while getting the license information from the registry. We'll therefore assume its an evaluation license.");
                Logger.Debug(ex);
                veeam.LicenseType = LicenseTypeEx.Evaluation;               
            }
            
            veeam.ProgramVersion = VeeamManager.VeeamVersion();

            if (veeam.ProgramVersion.StartsWith("9.0"))
            {
                veeam.CollectVmInformation90();
            }
            else if (veeam.ProgramVersion.StartsWith("9.5"))
            {
                veeam.CollectVmInformation95();
            }
            else
            {
                throw new AbpException("Unsupported version of Veeam detected. Please make sure the agent is the latest version");
            }

            veeam.ClientVersion = SettingManager.GetClientVersion();
            veeam.Edition = VeeamLicense.Edition;
            veeam.ExpirationDate = VeeamLicense.ExpirationDate;
            veeam.Id = await SettingManager.GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);
            veeam.SupportId = VeeamLicense.SupportId;
            veeam.TenantId = await SettingManager.GetSettingValueAsync<int>(SettingNames.AutotaskAccountId);

            veeam.Validate();
        }

        private static void CollectVmInformation90(this Veeam veeam)
        {
            veeam.vSphere = VeeamManager.GetAllVmInfos(EPlatform.EVmware).Count(item => item.State == EVmLicensingStatus.Managed);
            veeam.HyperV = VeeamManager.GetAllVmInfos(EPlatform.EHyperV).Count(item => item.State == EVmLicensingStatus.Managed);
        }

        private static void CollectVmInformation95(this Veeam veeam)
        {
            bool evaluation = veeam.LicenseType == LicenseTypeEx.Evaluation;

            VmsCounterInfo vSphereCounterInfo = VeeamManager.GetVmsCounters(EPlatform.EVmware, evaluation);
            VmsCounterInfo hypervCounterInfo = VeeamManager.GetVmsCounters(EPlatform.EHyperV, evaluation);

            veeam.vSphere = evaluation ? vSphereCounterInfo.TrialVmsCount : vSphereCounterInfo.NonTrialVmsCount;
            veeam.HyperV = evaluation ? hypervCounterInfo.TrialVmsCount : hypervCounterInfo.NonTrialVmsCount;
        }

        public static void Validate(this Veeam veeam)
        {
            if (veeam.ExpirationDate == default(DateTime))
            {
                throw new AbpException($"Invalid Expiration Date: {veeam.ExpirationDate}");
            }

            if (veeam.Id == default(Guid))
            {
                throw new AbpException($"Invalid Id: {veeam.Id}");
            }

            if (veeam.TenantId == default(int))
            {
                throw new AbpException($"Invalid Account: {veeam.TenantId}");
            }
        }
    }
}