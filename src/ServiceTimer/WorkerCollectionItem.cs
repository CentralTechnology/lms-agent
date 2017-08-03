namespace ServiceTimer
{
    using System.Threading;

    internal class WorkerCollectionItem
    {
        private WorkerCollectionItem()
        {
        }

        internal WorkerCollectionItem(TimerWorker worker)
        {
            worker.SignalEvent = new ManualResetEvent(false);

            Worker = worker;
        }

        /// <summary>
        ///     The worker
        /// </summary>
        internal TimerWorker Worker { get; set; }
    }
}