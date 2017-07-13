namespace Installer
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Service;
    using WixSharp;
    using WixSharp.Bootstrapper;
    using WixSharp.CommonTasks;

    class Script
    {
        static void Main(string[] args)
        {
            string productMsi = BuildMsi();

            var version = typeof(ServiceModule).Assembly.GetName().Version.ToString();

            Bundle bootstrapper = new Bundle(LicenseMonitoringSystemService.ServiceDisplayName)
            {
                Manufacturer = "Central Technology Ltd",
                OutDir = "bin/%Configuration%",
                OutFileName = "LmsInstaller",
                UpgradeCode = new Guid("dc9c2849-4c97-4f41-9174-d825ab335f9c"),
                Version = new Version(version),
                Chain = new List<ChainItem>
                {
                    new PackageGroupRef("NetFx452Redist"),
                    new ExePackage("../Resources/DotNetFramework/NDP452-KB2901907-x86-x64-AllOS-ENU.exe")
                    {
                        Id = "NetFx452FullExe",
                        Compressed = true,
                        PerMachine = true,
                        Permanent = true,
                        InstallCommand = "/q /norestart",
                        DetectCondition = "NOT WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED"
                    },
                    new MsiPackage(productMsi) {DisplayInternalUI = true}
                }
            };

            bootstrapper.Build();
        }
        static string BuildMsi()
        {
            File service;
            Project project = new Project("LMS",
                new Dir(@"%ProgramFiles%\License Monitoring System",
                    new DirPermission("LocalSystem", GenericPermission.Write | GenericPermission.Execute),
                    service = new File(@"%SolutionDir%/Service/bin/%Configuration%/LMS.exe"),
                    new DirFiles(@"%SolutionDir%/Service/bin/%Configuration%/*.*", f => !f.EndsWith("LMS.exe"))))
            {
                ControlPanelInfo = new ProductInfo
                {
                    HelpTelephone = "0845 413 88 99",
                    Manufacturer = "Central Technology Ltd",
                    NoModify = true,
                    NoRepair = true
                },
                InstallScope = InstallScope.perMachine,
                MajorUpgrade = new MajorUpgrade
                {
                    Schedule = UpgradeSchedule.afterInstallInitialize,
                    DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
                },
                Name = LicenseMonitoringSystemService.ServiceDisplayName,
                OutDir = "bin/%Configuration%",
                Platform = Platform.x64,
                UpgradeCode = new Guid("ADAC7706-188B-42E7-922B-50786779042A"),
                UI = WUI.WixUI_Common
            };

            project.SetVersionFrom("LMS.exe");
            project.SetNetFxPrerequisite("WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED");

            service.ServiceInstaller = new ServiceInstaller
            {
                DelayedAutoStart = true,
                Description = LicenseMonitoringSystemService.ServiceDescription,
                DisplayName = LicenseMonitoringSystemService.ServiceDisplayName,
                FirstFailureActionType = FailureActionType.restart,
                Name = LicenseMonitoringSystemService.ServiceName,
                RemoveOn = SvcEvent.Uninstall_Wait,
                ResetPeriodInDays = 1,
                RestartServiceDelayInSeconds = 30,
                SecondFailureActionType = FailureActionType.restart,
                ServiceSid = ServiceSid.none,
                StartOn = SvcEvent.Install,
                StopOn = SvcEvent.InstallUninstall_Wait,
                StartType = SvcStartType.auto,
                ThirdFailureActionType = FailureActionType.restart,
                Vital = true
            };

            return Compiler.BuildMsi(project);
        }
    }
}