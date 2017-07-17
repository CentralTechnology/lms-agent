namespace Service
{
    using System;
    using Abp;
    using Abp.Timing;
    using Castle.Facilities.Logging;
    using OneTrueError.Client;
    using Topshelf;

    class Runner
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var insightsUri = new Uri("https://insights.ct.co.uk/");
            OneTrue.Configuration.Credentials(insightsUri,
                "a779c7b1c4574095ab4843a96f16d4de",
                "6411f938a22f4c0c9d297ebbc8d04c7b");

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