namespace Core.Veeam
{
    using System;
    using System.Threading.Tasks;
    using Abp.Timing;
    using Administration;
    using Common.Client;
    using Common.Extensions;
    using Factory;
    using Models;
    using NLog;

    public class VeeamOrchestrator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly VeeamClient VeeamClient = new VeeamClient();

        private async Task<CallInStatus> GetStatus()
        {
            Guid device = await SettingFactory.SettingsManager().GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);
            return await VeeamClient.GetStatus(device);
        }

        public async Task Start()
        {
            CallInStatus status = await GetStatus();

            var veeam = new Veeam();

            Logger.Info("Collecting information...this could take some time.");

            await veeam.CollectInformation();

            Logger.Info(veeam.ToString());

            int uploadId = await VeeamClient.UploadId();
            veeam.UploadId = uploadId;
            veeam.CheckInTime = Clock.Now;
            veeam.Status = CallInStatus.CalledIn;

            if (status == CallInStatus.NeverCalledIn)
            {
                await VeeamClient.Add(veeam);
                return;
            }

            await VeeamClient.Update(veeam);
        }
    }
}