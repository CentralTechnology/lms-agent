namespace Service.Workers
{
    using Abp.Threading;
    using Core.Common.Extensions;
    using Core.Veeam;
    using ServiceTimer;

    internal class VeeamMonitorWorker : TimerWorker
    {
        protected VeeamManager VeeamManager;

        /// <summary>
        ///     30 second start up delay
        ///     10 second check
        ///     180*10/60 = 30 min execute
        /// </summary>
        internal VeeamMonitorWorker()
            : base(30000, 10000, 180)
        {
            FailedMessage = "************ Veeam Monitoring Failed ************";
            SuccessMessage = "************ Veeam Monitoring Successful ************";
            VeeamManager = new VeeamManager();
        }

        /// <inheritdoc />
        protected override void StartWork(TimerWorkerInfo info)
        {
        }

        /// <inheritdoc />
        protected override void Work(TimerWorkerInfo info)
        {
            RavenClient.AddTag("operation", "veeam");

            Logger.Info("Veeam monitoring begin...");

            if (!StartupManager.ValidateCredentials())
            {
                return;
            }

            bool veeamOnline = VeeamManager.VeeamOnline();
            if (!veeamOnline)
            {
                Logger.Error("Cannot contact the Veeam server. Please make sure all the Veeam services are started.");
                Logger.Error("We cannot go on like this.");
                Logger.Error(FailedMessage);
                return;
            }

            AsyncHelper.RunSync(() => new VeeamOrchestrator().Start());

            Logger.Info(SuccessMessage);
        }
    }
}