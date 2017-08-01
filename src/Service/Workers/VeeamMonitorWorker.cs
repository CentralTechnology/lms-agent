namespace Service.Workers
{
    using System;
    using Abp.Threading;
    using Core.Common.Constants;
    using Core.Factory;
    using ServiceTimer;
    using SharpRaven;
    using SharpRaven.Data;

    internal class VeeamMonitorWorker : TimerWorker
    {
        /// <summary>
        ///     30 second start up delay
        ///     10 second check
        ///     180*10/60 = 30 min execute
        /// </summary>
        internal VeeamMonitorWorker()
            : base(30000, 10000, 2)
        {
        }

        /// <inheritdoc />
        protected override void StartWork(TimerWorkerInfo info)
        {
            
        }

        /// <inheritdoc />
        protected override void Work(TimerWorkerInfo info)
        {
            Logger.Info("Veeam monitoring begin...");

            try
            {
                AsyncHelper.RunSync(() => OrchestratorFactory.VeeamOrchestrator().Start());

                Logger.Info("************ Veeam Monitoring Successful ************");
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error(ex.Message);
                Logger.Error("************ Veeam Monitoring Failed ************");                
            }
        }
    }
}