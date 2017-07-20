#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "LicenseMonitoringSystem",
                            repositoryOwner: "CentralTechnology",
                            repositoryName: "lms-agent",
                            appVeyorAccountName: "CentralTechnologyLtd",
                            shouldDownloadFullReleaseNotes: true,
                            shouldDownloadMilestoneReleaseNotes: true,
                            shouldPublishChocolatey: false,
                            shouldPublishNuGet: false,
                            shouldPublishGitHub: false,
                            shouldExecuteGitLink: false);

ToolSettings.SetToolSettings(context: Context,
                             buildPlatformTarget: PlatformTarget.x64);

Build.Run();