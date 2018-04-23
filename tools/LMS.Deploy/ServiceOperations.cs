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

            try
            {
                ServiceController service = new ServiceController(ServiceName);
                Log.Information($"Service is currently {service.Status}");
                if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
                {
                    Log.Information("Attempting to start the License Monitoring System service.");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                    Log.Information("Service successfully started.");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("License Monitoring System service was not found on this computer.");
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

            try
            {
                ServiceController service = new ServiceController(ServiceName);
                Log.Information($"Service is currently {service.Status}");
                if (service.Status == ServiceControllerStatus.Running)
                {
                    Log.Information("Attempting to stop the License Monitoring System service.");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));

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
            catch (Exception ex)
            {
                Log.Debug("License Monitoring System service was not found on this computer.");
                Log.Debug(ex, ex.Message);
            }
        }
    }
}