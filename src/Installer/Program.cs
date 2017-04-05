namespace Installer
{
    using System;
    using Microsoft.Deployment.WindowsInstaller;
    using WixSharp;
    using WixSharp.Bootstrapper;
    using WixSharp.CommonTasks;
    using Action = WixSharp.Action;

    class Script
    {
        static void Main(string[] args)
        {
            string productMsi = BuildMsi();

            var version = Environment.GetEnvironmentVariable("BuildVersion") ?? "1.0.0.0";

            Bundle bootstrapper = new Bundle("License Monitoring System",
                new PackageGroupRef("NetFx452Redist"),
                new ExePackage("../Resources/DotNetFramework/NDP452-KB2901907-x86-x64-AllOS-ENU.exe")
                {
                    Id = "NetFx452FullExe",
                    Compressed = true,
                    PerMachine = true,
                    Permanent = false,
                    InstallCommand = "/q /norestart /ChainingPackage FullX64Bootstrapper",
                    DetectCondition = "WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED='#0'"
                },
                new MsiPackage(productMsi) { DisplayInternalUI = true })
            {
                Manufacturer = "Central Technology Ltd",
                OutDir = "bin/%Configuration%",
                OutFileName = "LMS",
                UpgradeCode = new Guid("dc9c2849-4c97-4f41-9174-d825ab335f9c"),
                Version = new Version(version)          
            };            

            bootstrapper.Build();
        }
        static string BuildMsi()
        {
            Project project = new Project("LMS",
                new Dir(@"%ProgramFiles%\License Monitoring System",
                    new DirPermission("LocalSystem", GenericPermission.Write | GenericPermission.Execute),
                    new DirFiles(@"../Service/bin/%Configuration%/net452/*.*")))
            {
                Actions = new Action[]
                {
                  new InstalledFileAction("LMS.exe","install", Return.check,When.After,Step.InstallFinalize,Condition.NOT_Installed),
                  new ManagedAction(CustomActions.StartService,Return.check,When.After,Step.InstallFinalize,Condition.NOT_Installed),
                  new InstalledFileAction("LMS.exe","uninstall",Return.check,When.Before,Step.InstallFinalize, Condition.Installed)
                },
                ControlPanelInfo = new ProductInfo
                {
                    HelpTelephone = "0845 413 88 99",
                    Manufacturer = "Central Technology Ltd",
                    NoModify = true,
                    NoRepair = true
                },
                InstallScope = InstallScope.perMachine,
                Name = "License Monitoring System",
                OutDir = "bin/%Configuration%",
                Platform = Platform.x64,
                UpgradeCode = new Guid("ADAC7706-188B-42E7-922B-50786779042A"),
                UI = WUI.WixUI_Common
            };

            project.SetVersionFrom("LMS.exe");
            project.SetNetFxPrerequisite("WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED='#1'");

            return Compiler.BuildMsi(project);
        }
    }

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult StartService(Session session)
        {
            return session.HandleErrors(() =>
            {
                Tasks.StartService("LicenseMonitoringSystem");
            });
        }
    }
}