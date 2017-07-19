#load "tools/Cake.Recipe/Content/addins.cake"
#load "tools/Cake.Recipe/Content/analyzing.cake"
#load "tools/Cake.Recipe/Content/appveyor.cake"
#load "tools/Cake.Recipe/Content/build.cake"
#load "tools/Cake.Recipe/Content/chocolatey.cake"
#load "tools/Cake.Recipe/Content/codecov.cake"
#load "tools/Cake.Recipe/Content/coveralls.cake"
#load "tools/Cake.Recipe/Content/credentials.cake"
#load "tools/Cake.Recipe/Content/environment.cake"
#load "tools/Cake.Recipe/Content/gitlink.cake"
#load "tools/Cake.Recipe/Content/gitreleasemanager.cake"
#load "tools/Cake.Recipe/Content/gitter.cake"
#load "tools/Cake.Recipe/Content/gitversion.cake"
#load "tools/Cake.Recipe/Content/microsoftteams.cake"
#load "tools/Cake.Recipe/Content/nuget.cake"
#load "tools/Cake.Recipe/Content/packages.cake"
#load "tools/Cake.Recipe/Content/parameters.cake"
#load "tools/Cake.Recipe/Content/paths.cake"
#load "tools/Cake.Recipe/Content/slack.cake"
#load "tools/Cake.Recipe/Content/tasks.cake"
#load "tools/Cake.Recipe/Content/testing.cake"
#load "tools/Cake.Recipe/Content/tools.cake"
#load "tools/Cake.Recipe/Content/toolsettings.cake"
#load "tools/Cake.Recipe/Content/twitter.cake"
#load "tools/Cake.Recipe/Content/wyam.cake"

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