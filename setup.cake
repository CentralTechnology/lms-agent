#load "local:?path=tools/Cake.Recipe/Content/addins.cake"
#load "local:?path=tools/Cake.Recipe/Content/analyzing.cake"
#load "local:?path=tools/Cake.Recipe/Content/appveyor.cake"
#load "local:?path=tools/Cake.Recipe/Content/build.cake"
#load "local:?path=tools/Cake.Recipe/Content/chocolatey.cake"
#load "local:?path=tools/Cake.Recipe/Content/codecov.cake"
#load "local:?path=tools/Cake.Recipe/Content/coveralls.cake"
#load "local:?path=tools/Cake.Recipe/Content/credentials.cake"
#load "local:?path=tools/Cake.Recipe/Content/environment.cake"
#load "local:?path=tools/Cake.Recipe/Content/gitlink.cake"
#load "local:?path=tools/Cake.Recipe/Content/gitreleasemanager.cake"
#load "local:?path=tools/Cake.Recipe/Content/gitter.cake"
#load "local:?path=tools/Cake.Recipe/Content/gitversion.cake"
#load "local:?path=tools/Cake.Recipe/Content/microsoftteams.cake"
#load "local:?path=tools/Cake.Recipe/Content/nuget.cake"
#load "local:?path=tools/Cake.Recipe/Content/packages.cake"
#load "local:?path=tools/Cake.Recipe/Content/parameters.cake"
#load "local:?path=tools/Cake.Recipe/Content/paths.cake"
#load "local:?path=tools/Cake.Recipe/Content/slack.cake"
#load "local:?path=tools/Cake.Recipe/Content/tasks.cake"
#load "local:?path=tools/Cake.Recipe/Content/testing.cake"
#load "local:?path=tools/Cake.Recipe/Content/tools.cake"
#load "local:?path=tools/Cake.Recipe/Content/toolsettings.cake"
#load "local:?path=tools/Cake.Recipe/Content/twitter.cake"
#load "local:?path=tools/Cake.Recipe/Content/wyam.cake"


Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "LicenseMonitoringSystem",
                            repositoryOwner: "CentralTechnology",
                            repositoryName: "lms-agent",
							isPublicRepository: false,
                            appVeyorAccountName: "CentralTechnologyLtd",
                            shouldDownloadFullReleaseNotes: true,
                            shouldDownloadMilestoneReleaseNotes: true,
							shouldExecuteGitLink: false);

ToolSettings.SetToolSettings(context: Context,
                             buildPlatformTarget: PlatformTarget.x64);

Build.Run();