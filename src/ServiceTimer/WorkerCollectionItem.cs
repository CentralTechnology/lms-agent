/* 27 Oct 2014
// This code released to community under The Code Project Open License (CPOL) 1.02
// The copyright owner and author of this version of the code is Robert Ellis
// Please retain this notice and clearly identify your own edits, amendments and/or contributions
// In line with the CPOL this code is provided "AS IS" and without warranty
// Use entirely at your own risk
*/
namespace ServiceTimer
{
    using System;
    using System.Threading;

    internal class WorkerCollectionItem
    {
        /// <summary>
        /// The worker
        /// </summary>
        internal TimerWorker Worker { get; set; }
        
        private WorkerCollectionItem()
        {
        }

        internal WorkerCollectionItem(TimerWorker worker)
        {
            worker.signalEvent = new ManualResetEvent(false);

            Worker = worker;
        }
    }
}
