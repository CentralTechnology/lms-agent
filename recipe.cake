#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "LicenseMonitoringSystem",
                            repositoryOwner: "CentralTechnology",
                            repositoryName: "lms-agent",
							shouldRunGitVersion: true,
							shouldExecuteGitLink: false,
							isPublicRepository: false,
							shouldRunDupFinder: false,
							shouldRunInspectCode: false
							);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
							 dupFinderExcludePattern: new string[] { 
									Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tests/**/*.cs",  
									Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/tools/**/*.cs", 
									Context.MakeAbsolute(Context.Environment.WorkingDirectory) + "/src/LMS.Core/Connected Services/**/*.cs"},
							 testCoverageFilter: "+[*]* -[xunit.*]* -[*.Tests]* -[SharpRaven]* -[*]Portal.* -[*]Core.Migrations.* -[*]Migrations.* -[*]Actions.*"
							 );

var lmsSetup = BuildParameters.Paths.Directories.PublishedApplications.Combine("LMS.Installer/");
var lmsDeploy = BuildParameters.Paths.Directories.PublishedApplications.Combine("LMS.Deploy/");
var customArtifactsPath = BuildParameters.Paths.Directories.Build.Combine("Packages/");

Task("Copy-Custom-Files")
	.IsDependeeOf("Package")
	.Does(() => 
{	
		CleanDirectory(customArtifactsPath);
		CopyFiles(lmsSetup.CombineWithFilePath("LMS.Setup.exe").FullPath, customArtifactsPath);
		CopyFiles(lmsDeploy.CombineWithFilePath("Deploy.exe").FullPath, customArtifactsPath);
});

Task("Upload-AppVeyor-Artifacts-Custom-Files")
    .IsDependentOn("Package")
    .IsDependeeOf("Upload-AppVeyor-Artifacts")
    .WithCriteria(() => BuildParameters.IsRunningOnAppVeyor)
	.WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.Packages))
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

Build.Run();

