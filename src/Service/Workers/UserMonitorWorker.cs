namespace Service.Workers
{
    using System;
    using Abp.Threading;
    using Core.Factory;
    using ServiceTimer;
    using SharpRaven.Data;

    internal class UserMonitorWorker : TimerWorker
    {
        /// <summary>
        ///     30 second start up delay
        ///     10 second check
        ///     90*10/60 = 20 min execute
        /// </summary>
        internal UserMonitorWorker()
            : base(30000, 10000, 120)
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