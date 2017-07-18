﻿namespace Service.Workers
{
    using System;
    using Abp.Threading;
    using Core.Factory;
    using OneTrueError.Client;
    using ServiceTimer;

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
            Log.Info("User monitoring begin...");

            try
            {
                AsyncHelper.RunSync(() => OrchestratorFactory.Orchestrator().UserMonitor());

                Log.Info("************ User Monitoring Successful ************");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error("************ User Monitoring Failed ************");
                OneTrue.Report(ex);
            }
        }
    }
}