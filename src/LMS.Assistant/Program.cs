using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Assistant
{
    using System.Diagnostics;
    using System.IO;
    using System.ServiceProcess;

    class Program
    {
        static int Main(string[] args)
        {
            if (!File.Exists("lms.exe"))
            {
                WriteError("Assistant must be run from the same directory as LMS!");
                Continue();
                return 1;
            }

            ServiceController service = new ServiceController("LicenseMonitoringSystem");
            try
            {
                // checks the service exists
                WriteServiceStatus(service.Status);
            }
            catch (InvalidOperationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Continue();
                return 1;
            }

            if (service.Status == ServiceControllerStatus.Running)
            {
                Console.WriteLine("Attempting to stop the License Monitoring System Service");
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
                WriteServiceStatus(service.Status);
            }

            Console.WriteLine("Launching Program. This could take a while...");
            bool launched = Launch();
            if (!launched)
            {
                Continue();
                return 1;
            }

            Console.WriteLine("Attempting to start the License Monitoring System Service");
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
            if (service.Status == ServiceControllerStatus.Running)
            {
                WriteServiceStatus(service.Status);
                Console.WriteLine("Complete!");
            }
            else
            {
                WriteError("License Monitoring System Service didn't appear to start correctly. Please manually check the service.");
                Continue();
                return 1;
            }

            Continue();
            return 0;
        }

        static void Continue()
        {
            Console.WriteLine("Press [Enter] to coninue;");
            Console.ReadLine();
        }

        static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void WriteServiceStatus(ServiceControllerStatus status)
        {
            Console.Write("Current Status: ");
            switch (status)
            {
                case ServiceControllerStatus.ContinuePending:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ServiceControllerStatus.Paused:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ServiceControllerStatus.PausePending:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ServiceControllerStatus.Running:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ServiceControllerStatus.StartPending:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ServiceControllerStatus.Stopped:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ServiceControllerStatus.StopPending:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }

            Console.Write(status);
            Console.WriteLine();
            Console.ResetColor();
        }

        static bool Launch()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"lms.exe";
            startInfo.Arguments = "run -m Users";

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                Console.ResetColor();
                return false;
            }
        }
    }
}
