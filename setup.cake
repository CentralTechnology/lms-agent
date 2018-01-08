#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.Tasks.UploadAppVeyorArtifactsTask.Task.Actions.Clear();
BuildParameters.Tasks.UploadAppVeyorArtifactsTask
    .WithCriteria(() => BuildParameters.IsRunningOnAppVeyor)
    .Does(() => {
	Information("*** Upload files goes here. ***");
});

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
							 dupFinderExcludePattern: new string[] { Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tests/**/*.cs",  Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tools/**/*.cs", Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/src/Core/Connected Services/**/*.cs"},
							 testCoverageFilter: "+[*]* -[xunit.*]* -[*.Tests]* -[SharpRaven]*");

Build.Run();

