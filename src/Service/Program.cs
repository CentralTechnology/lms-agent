namespace LMS
{
    using Abp.Timing;
    using Core.Common.Constants;
    using Topshelf;

    class Runner
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Clock.Provider = ClockProviders.Utc;          

            HostFactory.Run(x =>
            {
                x.Service<LMSService>(sc =>
                {
                    sc.ConstructUsing(() => new LMSService());

                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());

                    sc.WhenPaused(s => s.Pause());
                    sc.WhenContinued(s => s.Continue());

                    sc.WhenShutdown(s => s.Shutdown());
                });

                x.UseLog4Net();
                x.RunAsLocalSystem();
                x.SetServiceName(Constants.ServiceName);
                x.SetDisplayName(Constants.ServiceDisplayName);
                x.SetDescription(Constants.ServiceDescription);
                x.StartAutomatically();
            });
        }
    }
}