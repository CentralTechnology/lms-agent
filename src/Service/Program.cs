namespace Service
{
    using Abp;
    using Abp.Timing;
    using Castle.Facilities.Logging;
    using Topshelf;

    class Runner
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Clock.Provider = ClockProviders.Utc;

            HostFactory.Run(sc =>
            {
                sc.Service<LicenseMonitoringSystemService>(s =>
                {
                    s.ConstructUsing(name => new LicenseMonitoringSystemService());
                    s.WhenStarted(lms => lms.Start());
                    s.WhenStopped(lms => lms.Stop());
                });

                sc.UseNLog();
                sc.RunAsLocalSystem();
                sc.SetServiceName(LicenseMonitoringSystemService.ServiceName);
                sc.SetDisplayName(LicenseMonitoringSystemService.ServiceDisplayName);
                sc.SetDescription(LicenseMonitoringSystemService.ServiceDescription);
                sc.StartAutomaticallyDelayed();
            });
        }
    }
}