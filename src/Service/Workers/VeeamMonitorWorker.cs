namespace Service.Workers
{
    using System;
    using System.IO;
    using Abp.Threading;
    using Core.Factory;
    using ServiceTimer;
    using SharpRaven.Data;

    internal class VeeamMonitorWorker : TimerWorker
    {
        private const string FailedMessage = "************ Veeam Monitoring Failed ************";
        private const string SuccessMessage = "************ Veeam Monitoring Successful ************";

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
            Logger.Info("Veeam monitoring begin...");

            try
            {
                AsyncHelper.RunSync(() => OrchestratorFactory.VeeamOrchestrator().Start());

                Logger.Info(SuccessMessage);
            }
            catch (IOException ex)
            {
                // chances are the reason this is thrown is because the client is low on disk space.
                // therefore we output to console instead of the logger.

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(FailedMessage);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                RavenClient.Capture(new SentryEvent(ex));
                Logger.Error(ex.Message);
                Logger.Error(FailedMessage);
            }
        }
    }
}