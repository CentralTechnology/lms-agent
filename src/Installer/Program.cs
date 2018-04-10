namespace Installer
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using WixSharp;
    using WixSharp.Bootstrapper;
    using WixSharp.CommonTasks;

    internal class Script
    {
        private static CustomConfiguration Configuration => GetConfiguration();

        private static string BuildMsi()
        {
            Console.WriteLine($"Solution Directory: {Configuration.SolutionDir}");
            Console.WriteLine($"Output Directory: {Configuration.OutDir}");
            Console.WriteLine($"Configuration: {Configuration.Configuration}");
            Console.WriteLine($"Service: {Configuration.SolutionDir}\\Service\\bin\\{Configuration.Configuration}\\net452\\win-x86\\LMS.exe");

            File service;
            var project = new Project("LMS",
                new Dir(@"%ProgramFiles%\License Monitoring System",
                    new DirPermission("LocalSystem", GenericPermission.All),
                    service = new File($"{Configuration.SolutionDir}\\Service\\bin\\{Configuration.Configuration}\\net452\\win-x86\\LMS.exe"),
                    new DirFiles($"{Configuration.SolutionDir}\\Service\\bin\\{Configuration.Configuration}\\net452\\win-x86\\*.*", f => !f.EndsWith("LMS.exe")))
            )
            {
                ControlPanelInfo = new ProductInfo
                {
                    HelpTelephone = "0845 413 88 99",
                    Manufacturer = "Central Technology Ltd",
                    NoModify = true,
                    NoRepair = true,
                    ProductIcon = "app_icon.ico"
                },
                InstallScope = InstallScope.perMachine,
                MajorUpgrade = new MajorUpgrade
                {
                    Schedule = UpgradeSchedule.afterInstallInitialize,
                    DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
                },
                Name = Constants.ServiceDisplayName,
                OutDir = Configuration.OutDir,
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

        private static CustomConfiguration GetConfiguration()
        {
            var file = System.IO.File.ReadAllText("C:\\temp\\configuration.json");
            return JsonConvert.DeserializeObject<CustomConfiguration>(file);
        }

        private static void Main(string[] args)
        {
            string product = BuildMsi();

            string version = Environment.GetEnvironmentVariable("GitVersion_AssemblySemVer") ?? System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var bootstrapper = new Bundle(Constants.ServiceDisplayName)
            {
                HelpTelephone = "0845 413 88 99",
                Manufacturer = "Central Technology Ltd",
                DisableModify = "yes",
                DisableRollback = true,
                IconFile = "app_icon.ico",
                OutDir = Configuration.OutDir,
                OutFileName = "LMS.Setup",
                UpgradeCode = new Guid("dc9c2849-4c97-4f41-9174-d825ab335f9c"),
                Version = new Version(version),
                Chain = new List<ChainItem>
                {
                    new PackageGroupRef("NetFx452Redist"),
                    new ExePackage
                    {
                        DetectCondition = "NOT WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED",
                        Id = "NetFx452WebExe",
                        InstallCommand = "/q /norestart /ChainingPackage LMS.Setup.exe",
                        Compressed = true,
                        SourceFile = "../Resources/DotNetFramework/NDP452-KB2901954-Web.exe",
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