namespace Service.Workers
{
    using System;
    using Core.Common.Extensions;
    using Core.Users;
    using ServiceTimer;

    internal class UserMonitorWorker : TimerWorker
    {
        /// <inheritdoc />
        /// <summary>
        ///     30 second start up delay
        ///     10 second check
        ///     90*10/60 = 20 min execute
        /// </summary>
        internal UserMonitorWorker()
            : base(30000, 10000, 120)
        {
            FailedMessage = "************ User Monitoring Failed ************";
            SuccessMessage = "************ User Monitoring Successful ************";
        }

        /// <inheritdoc />
        protected override void StartWork(TimerWorkerInfo info)
        {
        }

        /// <inheritdoc />
        protected override void Work(TimerWorkerInfo info)
        {
            RavenClient.AddTag("operation", "users");

            Logger.Info("User monitoring begin...");
            if (!StartupManager.ValidateCredentials())
            {
                return;
            }

            using (var orchestrator = new UserOrchestrator())
            {
                orchestrator.Start();
            }

            Console.WriteLine(Environment.NewLine);
            Logger.Info(SuccessMessage);
        }
    }
}