namespace LMS
{
    using System;
    using System.IO;
    using Abp.Timing;
    using CommandLine;
    using Serilog;
    using Serilog.Core;
    using Serilog.Exceptions;
    using Serilog.Formatting.Compact;
    using Topshelf;
    using Constants = Core.Constants.Constants;

    public class Program
    {
        public static LoggingLevelSwitch CurrentLogLevel { get; set; } = new LoggingLevelSwitch();

        static int Main(string[] args)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            if (Environment.UserInteractive)
            {
                Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight / 2);
            }
            
            Log.Logger = new LoggerConfiguration()
                         .Enrich.WithExceptionDetails()
                         .MinimumLevel.ControlledBy(CurrentLogLevel)
                         .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                         .WriteTo.Async(a => a.File(formatter: new RenderedCompactJsonFormatter(), path:"logs/log.txt", fileSizeLimitBytes: 8388608, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true))
                         .CreateLogger();

            if (Environment.UserInteractive && args != null)
            {
                Parser.Default.ParseArguments<UpdateOptions, RunOptions>(args)
                    .WithParsed<UpdateOptions>(ConsoleHost.Update)
                    .WithParsed<RunOptions>(ConsoleHost.Run);

                Console.WriteLine("Press [Enter] to exit.");
                Console.ReadLine();

                return 0;
            }

            return (int)HostFactory.Run(x =>
           {
               x.UseSerilog(Log.Logger);

               Clock.Provider = ClockProviders.Utc;

               x.Service<LMSService>(service =>
               {
                   service.ConstructUsing<LMSService>(s => new LMSService());

                   service.WhenStarted((tc, hostControl) => tc.Start(hostControl));
                   service.WhenStopped((tc, hostControl) => tc.Stop(hostControl));
               });

               x.SetStartTimeout(TimeSpan.FromSeconds(30));
               x.SetStopTimeout(TimeSpan.FromSeconds(120));

               x.RunAsLocalSystem();
               x.SetServiceName(Constants.ServiceName);
               x.SetDisplayName(Constants.ServiceDisplayName);
               x.SetDescription(Constants.ServiceDescription);
               x.StartAutomatically();

               x.OnException(exception => { Log.Logger.Error(exception, "An error has occurred."); });
           });
        }
    }
}