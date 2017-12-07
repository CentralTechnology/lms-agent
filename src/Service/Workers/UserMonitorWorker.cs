namespace LMS.Service.Workers
{
    using System;
    using Abp.Dependency;
    using LMS.Common.Extensions;
    using LMS.Startup;
    using ServiceTimer;
    using Users;

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
            using (var startupManager = IocManager.Instance.ResolveAsDisposable<IStartupManager>())
            {
                //if (!startupManager.Object.ValidateCredentials(TODO))
                //{
                //    return;
                //}
            }

            using (var userWorkerManager = IocManager.Instance.ResolveAsDisposable<IUserWorkerManager>())
            {
              //  userWorkerManager.Object.Start();
            }

            Console.WriteLine(Environment.NewLine);
            Logger.Info(SuccessMessage);
        }
    }
}