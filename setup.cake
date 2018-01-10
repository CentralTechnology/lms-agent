#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "LicenseMonitoringSystem",
                            repositoryOwner: "CentralTechnology",
                            repositoryName: "lms-agent",
							isPublicRepository: false,
                            appVeyorAccountName: "CentralTechnologyLtd",
							shouldExecuteGitLink: false,
							shouldDownloadMilestoneReleaseNotes: true);

ToolSettings.SetToolSettings(context: Context,
							 dupFinderExcludePattern: new string[] { Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tests/**/*.cs",  Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tools/**/*.cs", Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/src/Core/Connected Services/**/*.cs"},
							 testCoverageFilter: "+[*]* -[xunit.*]* -[*.Tests]* -[SharpRaven]* -[*]Portal.* -[*]Core.Migrations.* -[*]Migrations.* -[*]Actions.*");

Build.Run();

