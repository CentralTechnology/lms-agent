#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();
BuildParameters.Tasks.PublishGitHubReleaseTask.Task.Actions.Clear();
BuildParameters.Tasks.PublishGitHubReleaseTask
	.Does(() => {
		Information("Ignore");
	});

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
							 dupFinderExcludePattern: new string[] { Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tests/**/*.cs",  Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tools/**/*.cs", Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/src/LMS.Core/Connected Services/**/*.cs"},
							 testCoverageFilter: "+[*]* -[xunit.*]* -[*.Tests]* -[SharpRaven]* -[*]Portal.* -[*]Core.Migrations.* -[*]Migrations.* -[*]Actions.*");

var lmsSetup = BuildParameters.Paths.Directories.PublishedApplications.Combine("LMS.Installer/");
var lmsDeploy = BuildParameters.Paths.Directories.PublishedApplications.Combine("LMS.Deploy/");
var customArtifactsPath = BuildParameters.Paths.Directories.Build.Combine("Packages/Custom");

Task("Copy-Custom-Files")
	.IsDependeeOf("Package")
	.Does(() => 
{	
		CleanDirectory(customArtifactsPath);
		CopyFiles(lmsSetup.CombineWithFilePath("LMS.Setup.exe").FullPath, customArtifactsPath);
		CopyFiles(lmsDeploy.CombineWithFilePath("LMS.Deploy.exe").FullPath, customArtifactsPath);
});

Task("Upload-AppVeyor-Artifacts-Custom-Files")
    .IsDependentOn("Package")
    .IsDependeeOf("Upload-AppVeyor-Artifacts")
    .WithCriteria(() => BuildParameters.IsRunningOnAppVeyor)
    .Does(() =>
{
    foreach(var package in GetFiles(customArtifactsPath + "/*"))
    {
        AppVeyor.UploadArtifact(
			package,
			new AppVeyorUploadArtifactsSettings
			{
				DeploymentName = package.GetFilenameWithoutExtension().ToString()
			});
    }
});

Task("Publish-GitHub-Release-Custom-Files")
    .IsDependentOn("Package")
    .IsDependentOn("Copy-Custom-Files")
    .IsDependeeOf("Publish-GitHub-Release")
    .WithCriteria(() => BuildParameters.ShouldPublishGitHub)
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            foreach(var package in GetFiles(customArtifactsPath + "/*"))
            {
                GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
            }
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
)
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});

Build.Run();

