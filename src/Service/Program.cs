namespace LMS.Service
{
    using System;
    using Abp.Timing;
    using CommandLine;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Topshelf;
    using Constants = Core.Extensions.Constants.Constants;

    class Program
    {
        static int Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight / 2);

            if (args != null && args.Length > 0)
            {
                Parser.Default.ParseArguments<UpdateOptions, RunOptions>(args)
                    .WithParsed<UpdateOptions>(ConsoleHost.Update)
                    .WithParsed<RunOptions>(ConsoleHost.Run);

                return 0;
            }

            return (int)HostFactory.Run(x =>
           {
               x.UseSerilog();

               Clock.Provider = ClockProviders.Utc;

               x.Service<LMSService>(service =>
               {
                   service.ConstructUsing<LMSService>(s => new LMSService());

                   service.WhenStarted((tc, hostControl) => tc.Start(hostControl));
                   service.WhenStopped((tc, hostControl) => tc.Stop(hostControl));
               });

               x.SetStartTimeout(TimeSpan.FromSeconds(15));
               x.SetStopTimeout(TimeSpan.FromSeconds(60));

               x.RunAsLocalSystem();
               x.SetServiceName(Constants.ServiceName);
               x.SetDisplayName(Constants.ServiceDisplayName);
               x.SetDescription(Constants.ServiceDescription);
               x.StartAutomatically();

               x.OnException(exception =>
               {
                   Console.WriteLine($@"Exception thrown - {exception.Message}");
                   Console.ReadLine();
               });
           });
        }
    }
}