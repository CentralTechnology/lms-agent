namespace Service
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public class LicensingMonitoringInstaller : Installer
    {
        public LicensingMonitoringInstaller()
        {
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "License Monitoring System";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}