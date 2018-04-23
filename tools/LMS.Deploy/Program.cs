namespace Deploy
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using LMS.Deploy;
    using Serilog;
    using Serilog.Events;

    class Program
    {
        static async Task Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Logger(config => config
                .MinimumLevel.Verbose()
                .Filter.ByExcluding(e => e.Level == LogEventLevel.Debug)
                .WriteTo.Console())
                .WriteTo.Logger(config => config
                .MinimumLevel.Verbose()
                .Filter.ByExcluding(e => e.Level == LogEventLevel.Verbose)
                .WriteTo.File("deploy.txt"))
                .CreateLogger();

            Log.Information("Starting deployment....");

            try
            {
                var deploymentOperations = new DeploymentOperations();
                await deploymentOperations.StartAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Error("************ Deployment  Failed ************");
                throw;
            }

            Log.Information("************ Deployment  Successful ************");

            Log.Debug("Cleaning up old installations.");
            if (Environment.Is64BitOperatingSystem)
            {
                var programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                if (!string.IsNullOrEmpty(programFiles))
                {
                    var oldInstallPath = Path.Combine(programFiles, "License Monitoring System");
                    if (Directory.Exists(oldInstallPath))
                    {
                        Log.Information($"Old installation detected at: {oldInstallPath}");
                        Log.Information("Attempting to delete the directory.");

                        try
                        {
                            Directory.Delete(oldInstallPath, true);
                            Log.Information("Deletion successful.");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Deletion failed - {ex.Message}");
                            Log.Debug(ex, ex.Message);
                        }
                    }
                }
               
            }

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Press [Enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}