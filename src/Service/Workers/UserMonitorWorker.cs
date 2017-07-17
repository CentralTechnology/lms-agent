using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Workers
{
    using Abp.Threading;
    using Core.Factory;
    using ServiceTimer;

    internal class UserMonitorWorker : TimerWorker
    {

        internal UserMonitorWorker()
            : base(delayOnStart: 30000, timerInterval: 10000, workOnElapseCount: 90)
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
                OneTrueError.Client.OneTrue.Report(ex);
            }
            
        }
    }
}
