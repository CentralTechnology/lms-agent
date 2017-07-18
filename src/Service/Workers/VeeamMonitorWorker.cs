namespace Service.Workers
{
    using System;
    using Abp.Threading;
    using Core.Factory;
    using OneTrueError.Client;
    using ServiceTimer;

    internal class VeeamMonitorWorker : TimerWorker
    {
        /// <summary>
        ///     30 second start up delay
        ///     10 second check
        ///     180*10/60 = 30 min execute
        /// </summary>
        internal VeeamMonitorWorker()
            : base(30000, 10000, 180)
        {
        }

        /// <inheritdoc />
        protected override void StartWork(TimerWorkerInfo info)
        {
            
        }

        /// <inheritdoc />
        protected override void Work(TimerWorkerInfo info)
        {
            Log.Info("Veeam monitoring begin...");

            try
            {
                AsyncHelper.RunSync(() => OrchestratorFactory.Orchestrator().VeeamMonitor());

                Log.Info("************ Veeam Monitoring Successful ************");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error("************ Veeam Monitoring Failed ************");
                OneTrue.Report(ex);
            }
        }
    }
}