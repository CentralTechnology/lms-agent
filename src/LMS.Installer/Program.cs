namespace Installer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using WixSharp;
    using WixSharp.Bootstrapper;
    using WixSharp.CommonTasks;

    [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
    internal class Script
    {
        private static Version _version;
        private static string BuildServiceMsi()
        {
            File service;
            var project = new ManagedProject("LMS",
                new Dir(@"%ProgramFiles%\License Monitoring System",
                    new DirPermission("LocalSystem", GenericPermission.All),
                    service = new File(new Id("LMS_file"),"%SolutionDir%\\LMS.Service\\bin\\%Configuration%\\LMS.exe"),
                    new DirFiles("%SolutionDir%\\LMS.Service\\bin\\%Configuration%\\*.*", f => !f.EndsWith("LMS.exe")))
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
                Name = Constants.ServiceDisplayName,
                OutDir = "bin\\%Configuration%",
                UI = WUI.WixUI_Minimal,
                GUID = new Guid("ADAC7706-188B-42E7-922B-50786779042A"),
                RebootSupressing = RebootSupressing.ReallySuppress                
            };

            project.BeforeInstall += Project_BeforeInstall;
            project.SetVersionFrom("LMS_file");
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
            
            project.MajorUpgrade = new MajorUpgrade
            {
                Schedule = UpgradeSchedule.afterInstallInitialize,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };

            _version = project.Version;
            return Compiler.BuildMsi(project);
        }

        private static void Project_BeforeInstall(SetupEventArgs e)
        {
            if (e.IsInstalling || e.IsUpgrading)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(e.InstallDir, "Configuration.sdf")))
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(e.InstallDir, "Data")))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(e.InstallDir, "Data"));
                    }

                    System.IO.File.Move(System.IO.Path.Combine(e.InstallDir, "Configuration.sdf"), System.IO.Path.Combine(System.IO.Path.Combine(e.InstallDir, "Data"), "Configuration.sdf"));
                    System.IO.File.Delete(System.IO.Path.Combine(e.InstallDir, "Configuration.sdf"));
                }
                else
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(e.InstallDir, "Data")))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(e.InstallDir, "Data"));
                    }
                }
            }
        }

        private static string BuildGuiMsi()
    {
        File gui;
            var project = new Project("LMS Configuration",
                new Dir(@"%ProgramFiles%\License Monitoring System\\Configuration",
                    new DirPermission("LocalSystem", GenericPermission.All),
                        gui = new File(new Id("LMS_UI_File"),"%SolutionDir%\\LMS.Gui\\bin\\%Configuration%\\Configuration.exe"),
                    new DirFiles("%SolutionDir%\\LMS.Gui\\bin\\%Configuration%\\*.*", f => !f.EndsWith("Configuration.exe")))
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
                Name = Constants.ServiceDisplayName + " - Configuration",
                OutDir = "bin\\%Configuration%",
                UI = WUI.WixUI_Minimal,
                GUID = new Guid("4bbed23e-c112-4e20-a4d0-eae29429d4d7"),
                RebootSupressing = RebootSupressing.ReallySuppress
                
            };

        gui.Shortcuts = new[]
        {
            new FileShortcut("LMS Configuration", @"%Desktop%"){ IconFile = @"app_icon.ico", Name = "LMS Configuration" }
        };

            project.SetVersionFrom("LMS_UI_file");
            project.SetNetFxPrerequisite("WIX_IS_NETFRAMEWORK_452_OR_LATER_INSTALLED");
            project.CustomIdAlgorithm = project.HashedTargetPathIdAlgorithm;

            project.MajorUpgrade = new MajorUpgrade
            {
                Schedule = UpgradeSchedule.afterInstallInitialize,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };

            return Compiler.BuildMsi(project);
        }

        private static void Main(string[] args)
        {

            string product = BuildServiceMsi();
            string gui = BuildGuiMsi();

            string version = Environment.GetEnvironmentVariable("GitVersion_AssemblySemVer") ?? System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

#pragma warning disable IDE0017 // Simplify object initialization
            var bootstrapper = new Bundle(Constants.ServiceDisplayName,
                new PackageGroupRef("NetFx452Web"),
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
                    Compressed = true,
                    DisplayInternalUI = true,
                    Visible = false
                },
                new MsiPackage(gui)
                {
                    Compressed = true,
                    DisplayInternalUI = true,
                    Visible = false
                }
            );
#pragma warning restore IDE0017 // Simplify object initialization

            bootstrapper.SuppressWixMbaPrereqVars = true; // NetFx452Web also defines WixMbaPrereqVars
            bootstrapper.Version = new Version(version);
            bootstrapper.UpgradeCode = new Guid("dc9c2849-4c97-4f41-9174-d825ab335f9c");
            bootstrapper.OutDir = "bin\\%Configuration%";
            bootstrapper.OutFileName = "LMS.Setup";
            bootstrapper.IconFile = "app_icon.ico";
            bootstrapper.HelpTelephone = "0845 413 88 99";
            bootstrapper.Manufacturer = "Central Technology Ltd";

            // the following two assignments will hide Bundle entry form the Programs and Features (also known as Add/Remove Programs)
            bootstrapper.DisableModify = "yes";
          //  bootstrapper.DisableRemove = true;

            bootstrapper.PreserveTempFiles = true;

            bootstrapper.Validate();
            bootstrapper.Build();
        }
    }
}