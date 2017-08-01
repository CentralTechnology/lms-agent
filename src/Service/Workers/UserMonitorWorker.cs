namespace Service.Workers
{
    using System;
    using Abp.Threading;
    using Core.Common.Constants;
    using Core.Factory;
    using ServiceTimer;
    using SharpRaven;
    using SharpRaven.Data;

    internal class UserMonitorWorker : TimerWorker
    {
        /// <summary>
        /// 30 second start up delay
        /// 10 second check
        /// 90*10/60 = 15 min execute
        /// </summary>
        internal UserMonitorWorker()
            : base(30000, 10000, 90)
        {
        }

        /// <inheritdoc />
        protected override void StartWork(TimerWorkerInfo info)
        {
        }

        /// <inheritdoc />
        protected override void Work(TimerWorkerInfo info)
        {
            Logger.Info("User monitoring begin...");

            try
            {
                AsyncHelper.RunSync(() => OrchestratorFactory.Orchestrator().UserMonitor());

                Logger.Info("************ User Monitoring Successful ************");
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error(ex.Message);
                Logger.Error("************ User Monitoring Failed ************");
            }
        }
    }
}