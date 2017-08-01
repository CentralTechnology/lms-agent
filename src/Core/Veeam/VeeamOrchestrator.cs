using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam
{
    using Administration;
    using Common.Extensions;
    using Factory;
    using Models;
    using NLog;

    public class VeeamOrchestrator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task Start()
        {
            var status = await GetStatus();

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

            // collect information
        }

        public async Task<CallInStatus> GetStatus()
        {
            var device = await SettingFactory.SettingsManager().GetSettingValueAsync<Guid>(SettingNames.CentrastageDeviceId);
            return await ClientFactory.VeeamClient().GetStatus(device);
        }
    }
}
