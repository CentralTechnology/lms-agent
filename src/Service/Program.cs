namespace Service
{
    using Abp;
    using Abp.Topshelf;
    using Castle.Facilities.Logging;
    using Topshelf;

    class Runner
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AbpBootstrapper bootstrapper = AbpBootstrapper.Create<ServiceModule>())
            {
                bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("NLog.config"));
                bootstrapper.Initialize();

                HostFactory.Run(serviceConfig =>
                {
                    serviceConfig.UseAbp(bootstrapper);
                    serviceConfig.RunAsLocalSystem();
                    serviceConfig.SetServiceName("LicenseMonitoringSystem");
                    serviceConfig.SetDisplayName("License Monitoring System");
                    serviceConfig.SetDescription("A tool used to monitor various licenses.");
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