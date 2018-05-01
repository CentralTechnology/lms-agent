namespace LMS.Deploy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using Serilog;

    public class ServiceOperations
    {
        private const string ServiceName = "LicenseMonitoringSystem";
        public void Start()
        {
            if (!Exists())
            {
                Log.Debug("Service does not exist.");
                return;
            }

            ServiceController service;

            try
            {
                service = new ServiceController(ServiceName);
                Log.Information($"Service is currently {service.Status}");
            }
            catch (InvalidOperationException ex)
            {
                Log.Debug("License Monitoring System service was not found on this computer.");
                Log.Debug(ex, ex.Message);
                return;
            }

            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    Log.Information("Service successfully started.");
                    return;
                }

                Log.Information("Attempting to start the License Monitoring System service.");
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                Log.Information("Service successfully started.");

            }
            catch (InvalidOperationException ex)
            {
                Log.Error($"License Monitoring System service cannot be started - {ex.Message}");
                Log.Debug(ex, ex.Message);
            }
        }

        private static bool Exists()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName.Equals(ServiceName));
        }

        public void Stop()
        {
            if (!Exists())
            {
                Log.Debug("Service does not exist.");
                return;
            }

            ServiceController service;

            try
            {
                service = new ServiceController(ServiceName);
                Log.Information($"Service is currently {service.Status}");
            }
            catch (InvalidOperationException ex)
            {
                Log.Debug("License Monitoring System service was not found on this computer.");
                Log.Debug(ex, ex.Message);
                return;
            }

            try
            {

                if (service.Status == ServiceControllerStatus.Running)
                {
                    Log.Information("Attempting to stop the License Monitoring System service.");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                    Log.Information($"Service is now {service.Status}");

                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        Log.Information("Service has failed to stop in a reasonable amount of time.");
                        Log.Information("Killing the process");
                        foreach (var process in Process.GetProcessesByName("LMS"))
                        {
                            process.Kill();
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Log.Error($"License Monitoring System service cannot be stopped - {ex.Message}");
                Log.Debug(ex, ex.Message);
                throw;
            }
        }
    }
}