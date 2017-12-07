namespace LMS.Service.Workers
{
    using System;
    using Abp.Dependency;
    using LMS.Common.Extensions;
    using LMS.Startup;
    using ServiceTimer;
    using Veeam;
    using Veeam.Managers;

    internal class VeeamMonitorWorker : TimerWorker
    {
        /// <inheritdoc />
        /// <summary>
        ///     30 second start up delay
        ///     10 second check
        ///     180*10/60 = 30 min execute
        /// </summary>
        internal VeeamMonitorWorker()
            : base(30000, 10000, 120)
        {
            FailedMessage = "************ Veeam Monitoring Failed ************";
            SuccessMessage = "************ Veeam Monitoring Successful ************";
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
            using (var startupManager = IocManager.Instance.ResolveAsDisposable<IStartupManager>())
            {
                if (!startupManager.Object.ValidateCredentials())
                {
                    return;
                }
            }

            using (var veeamManager = IocManager.Instance.ResolveAsDisposable<IVeeamManager>())
            {
                bool veeamOnline = veeamManager.Object.IsOnline();
                if (!veeamOnline)
                {
                    Logger.Error("Cannot contact the Veeam server. Please make sure all the Veeam services are started.");
                    Logger.Error("We cannot go on like this.");
                    Logger.Error(FailedMessage);
                    return;
                }
            }


            using (var veeamWorkerManager = IocManager.Instance.ResolveAsDisposable<IVeeamWorkerManager>())
            {
                veeamWorkerManager.Object.Start();

            }

            Console.WriteLine(Environment.NewLine);
            Logger.Info(SuccessMessage);
        }
    }
}