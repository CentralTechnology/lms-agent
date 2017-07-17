﻿namespace ServiceTimer
{
    using System.Threading;

    internal class WorkerCollectionItem
    {
        private WorkerCollectionItem()
        {
        }

        internal WorkerCollectionItem(TimerWorker worker)
        {
            worker.signalEvent = new ManualResetEvent(false);

            Worker = worker;
        }

        /// <summary>
        ///     The worker
        /// </summary>
        internal TimerWorker Worker { get; set; }
    }
}