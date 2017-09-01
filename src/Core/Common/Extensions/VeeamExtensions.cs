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

            Version programVersion = Version.Parse(veeam.ProgramVersion);

            veeam.CollectVmInformation(programVersion);
            veeam.ClientVersion = SettingManager.GetClientVersion();
            veeam.Edition = VeeamLicense.Edition;
            veeam.ExpirationDate = VeeamLicense.ExpirationDate;
            veeam.Id = await SettingManager.GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);
            veeam.SupportId = VeeamLicense.SupportId;
            veeam.TenantId = await SettingManager.GetSettingValueAsync<int>(SettingNames.AutotaskAccountId);

            veeam.Validate();
        }

        /// <summary>
        ///     Top level function that determines which function to call for a particular version of Veeam.
        /// </summary>
        /// <param name="veeam"></param>
        /// <param name="programVersion"></param>
        private static void CollectVmInformation(this Veeam veeam, Version programVersion)
        {
            if (programVersion.Major == 9 && programVersion.Minor == 5)
            {
                if (programVersion.Revision == 1038)
                {
                    veeam.CollectVmInformation9501038();                   
                    return;
                }

                veeam.CollectVmInformation95();
                return;
            }

            // default
            veeam.CollectVmInformation90();
        }

        /// <summary>
        ///     Collect VM count for Veeam 9.0.X.X
        /// </summary>
        /// <param name="veeam"></param>
        private static void CollectVmInformation90(this Veeam veeam)
        {
            veeam.vSphere = VeeamManager.GetAllVmInfos(EPlatform.EVmware).Count(item => item.State == EVmLicensingStatus.Managed);
            veeam.HyperV = VeeamManager.GetAllVmInfos(EPlatform.EHyperV).Count(item => item.State == EVmLicensingStatus.Managed);
        }

        /// <summary>
        ///     Collect VM count for Veeam 9.5.X.X
        /// </summary>
        /// <param name="veeam"></param>
        private static void CollectVmInformation95(this Veeam veeam)
        {
            bool evaluation = veeam.LicenseType == LicenseTypeEx.Evaluation;

            VmsCounterInfo vSphereCounterInfo = VeeamManager.GetVmsCounters(EPlatform.EVmware, evaluation);
            VmsCounterInfo hypervCounterInfo = VeeamManager.GetVmsCounters(EPlatform.EHyperV, evaluation);

            veeam.vSphere = evaluation ? vSphereCounterInfo.TrialVmsCount : vSphereCounterInfo.NonTrialVmsCount;
            veeam.HyperV = evaluation ? hypervCounterInfo.TrialVmsCount : hypervCounterInfo.NonTrialVmsCount;
        }

        /// <summary>
        ///     Collect VM count for Veeam 9.5.0.1038
        /// </summary>
        /// <param name="veeam"></param>
        private static void CollectVmInformation9501038(this Veeam veeam)
        {
            veeam.vSphere = VeeamManager.GetProtectedVmsCount(EPlatform.EVmware);
            veeam.HyperV = VeeamManager.GetProtectedVmsCount(EPlatform.EHyperV);
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