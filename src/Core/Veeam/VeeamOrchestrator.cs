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

        public async Task<CallInStatus> GetStatus()
        {
            Guid device = await SettingFactory.SettingsManager().GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);
            return await VeeamClient.GetStatus(device);
        }

        public async Task Start()
        {
            CallInStatus status = await GetStatus();

            switch (status)
            {
                case CallInStatus.CalledIn:
                    Logger.Info("Client is called in - Skipping.");
                    return;
                case CallInStatus.NotCalledIn:
                    Logger.Info("Client has not called in.");
                    break;
                case CallInStatus.NeverCalledIn:
                    Logger.Info("Client has never called in.");
                    break;
                default:
                    Logger.Info("Client has never called in.");
                    break;
            }

            var veeam = new Veeam();

            Logger.Info("Collecting information...this could take some time.");

            await veeam.CollectInformation();

            Logger.Info(veeam.ToString());

            var uploadId = await VeeamClient.UploadId();
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