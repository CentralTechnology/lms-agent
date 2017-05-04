namespace Service
{
    using System;
    using Abp;
    using Abp.Dependency;
    using Abp.Timing;
    using Abp.Topshelf;
    using Castle.Facilities.Logging;
    using Core.Administration;
    using Topshelf;

    class Runner
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Clock.Provider = ClockProviders.Utc;

            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<ServiceModule>())
            {
                bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("NLog.config"));
                bootstrapper.Initialize();

                HostFactory.Run(serviceConfig =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    serviceConfig.UseAbp(bootstrapper);
                    serviceConfig.RunAsLocalSystem();
                    serviceConfig.SetServiceName(LicenseMonitoringSystemService.ServiceName);
                    serviceConfig.SetDisplayName(LicenseMonitoringSystemService.ServiceDisplayName);
                    serviceConfig.SetDescription(LicenseMonitoringSystemService.ServiceDescription);
                    serviceConfig.StartAutomaticallyDelayed();

                    serviceConfig.Service<LicenseMonitoringSystemService>(serviceInstance =>
                    {
                        serviceInstance.ConstructUsingAbp();

                        serviceInstance.WhenStarted(execute => execute.Start());

                        serviceInstance.WhenStopped(execute => execute.Stop());
                    });

                });
            }
        }
    }
}