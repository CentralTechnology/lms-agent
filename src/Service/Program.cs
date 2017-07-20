namespace Service
{
    using System;
    using Abp.Timing;
    using Core.Common.Constants;
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

            HostFactory.Run(x =>
            {
                x.Service<LmsService>(sc =>
                {
                    sc.ConstructUsing(() => new LmsService());

                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());

                    sc.WhenPaused(s => s.Pause());
                    sc.WhenContinued(s => s.Continue());

                    sc.WhenShutdown(s => s.Shutdown());
                });

                x.UseNLog();
                x.RunAsLocalSystem();
                x.SetServiceName(Constants.ServiceName);
                x.SetDisplayName(Constants.ServiceDisplayName);
                x.SetDescription(Constants.ServiceDescription);
                x.StartAutomatically();
            });
        }
    }
}