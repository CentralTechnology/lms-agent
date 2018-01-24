namespace Installer
{
    using System;
    using System.Collections.Generic;
    using WixSharp;
    using WixSharp.Bootstrapper;
    using WixSharp.CommonTasks;

    class Script
    {
        static string BuildMsi()
        {
            File service;
            var project = new Project("LMS",
                new Dir(@"%ProgramFiles%\License Monitoring System",
                    new DirPermission("LocalSystem", GenericPermission.All),
                    service = new File(@"%SolutionDir%/Service/bin/%Configuration%/LMS.exe"),
                    new DirFiles(@"%SolutionDir%/Service/bin/%Configuration%/*.*", f => !f.EndsWith("LMS.exe")))
            )
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
                Name = Constants.ServiceDisplayName,
                OutDir = "bin/%Configuration%",
                UpgradeCode = new Guid("ADAC7706-188B-42E7-922B-50786779042A"),
                UI = WUI.WixUI_Common
            };

            project.ExtractVersionFrom("LMS.exe");
            project.SetNetFxPrerequisite("WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED");
            project.CustomIdAlgorithm = project.HashedTargetPathIdAlgorithm;
            service.ServiceInstaller = new ServiceInstaller
            {
                DelayedAutoStart = true,
                Description = Constants.ServiceDescription,
                DisplayName = Constants.ServiceDisplayName,
                FirstFailureActionType = FailureActionType.restart,
                Name = Constants.ServiceName,
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

        static void Main(string[] args)
        {
            string product = BuildMsi();

            string version = Environment.GetEnvironmentVariable("GitVersion_AssemblySemVer") ?? System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var bootstrapper = new Bundle(Constants.ServiceDisplayName)
            {
                Manufacturer = "Central Technology Ltd",
                OutDir = "bin/%Configuration%",
                OutFileName = "LMS.Setup",
                UpgradeCode = new Guid("dc9c2849-4c97-4f41-9174-d825ab335f9c"),
                Version = new Version(version),
                Chain = new List<ChainItem>
                {
                    new PackageGroupRef("NetFx452Redist"),
                    new ExePackage
                    {
                        DetectCondition = "NOT WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED",
                        Id = "NetFx452FullExe",
                        InstallCommand = "/q /norestart",
                        Compressed = true,
                        SourceFile = "../Resources/DotNetFramework/NDP452-KB2901907-x86-x64-AllOS-ENU.exe",
                        PerMachine = true,
                        Permanent = true,
                        Vital = true
                    },
                    new ExePackage
                    {
                        Compressed = true,
                        InstallCommand = "/i /qb",
                        InstallCondition = "VersionNT64",
                        PerMachine = true,
                        Permanent = true,
                        SourceFile = "../Resources/SQLCompact/SSCERuntime_x64-ENU.exe",
                        Vital = true
                    },
                    new ExePackage
                    {
                        Compressed = true,
                        InstallCommand = "/i /qb",
                        InstallCondition = "NOT VersionNT64",
                        PerMachine = true,
                        Permanent = true,
                        SourceFile = "../Resources/SQLCompact/SSCERuntime_x86-ENU.exe",
                        Vital = true
                    },
                    new MsiPackage(product)
                    {
                        DisplayInternalUI = true
                    }
                }
            };

            bootstrapper.Build();
        }
    }
}