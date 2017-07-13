$psake.use_exit_on_error = $true
properties {
	$config = 'Debug';
	$platform = 'Any CPU';
	$nugetExe = "..\Tools\NuGet\NuGet.exe";
	$projectName = "LicenseMonitoringSystem";
}

$private = "This is a private task not meant for external use!";

function get-buildArtifactsDirectory {
	return "." | Resolve-Path | Join-Path -ChildPath "../BuildArtifacts";
}

function get-buildScriptsDirectory {
	return "." | Resolve-Path;
}

function get-sourceDirectory {
	return "." | Resolve-Path | Join-Path -ChildPath "../src";
}

function get-rootDirectory {
	return "." | Resolve-Path | Join-Path -ChildPath "../";
}

function create-PackageDirectory( [Parameter(ValueFromPipeline=$true)]$packageDirectory ) {
	process {
		Write-Verbose "checking for package path $packageDirectory...";
		if( !(Test-Path $packageDirectory ) ) {
			Write-Verbose "creating package directory at $packageDirectory...";
			mkdir $packageDirectory | Out-Null;
		}
	}
}

function remove-PackageDirectory( [Parameter(ValueFromPipeline=$true)]$packageDirectory ) {
	process {
		Write-Verbose "Checking directory at $packageDirectory...";
		if(Test-Path $packageDirectory) {
			Write-Verbose "Removing directory at $packageDirectory...";
			Remove-Item $packageDirectory -recurse -force;
		}
	}
}

function test-CommandExists {
  Param ($command);
  $oldPreference = $ErrorActionPreference;

  $ErrorActionPreference = 'stop';

  try
  {
    Write-Output "Testing command...";
    if(Get-Command $command){
      RETURN $true;
    }
  } catch {
    Write-Output "$command does not exist";
    RETURN $false;
  } finally {
    $ErrorActionPreference=$oldPreference;
  }
}

function isAppVeyor() {
	Test-Path -Path env:\APPVEYOR
}

function isChocolateyInstalled() {
	$script:chocolateyDir = $null
	if ($env:ChocolateyInstall -ne $null) {
		$script:chocolateyDir = $env:ChocolateyInstall;
	} elseif (Test-Path (Join-Path $env:SYSTEMDRIVE Chocolatey)) {
		$script:chocolateyDir = Join-Path $env:SYSTEMDRIVE Chocolatey;
	} elseif (Test-Path (Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey)) {
		$script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey;
	}

	Test-Path -Path $script:chocolateyDir;
}

function analyseDupFinderResults( [Parameter(ValueFromPipeline=$true)]$dupFinderResultsFile ) {
	$dupFinderErrors = [xml](Get-Content $dupFinderResultsFile);
	$anyFailures = $FALSE

	foreach ($duplicateCost in $dupFinderErrors.DuplicatesReport.Duplicates.Duplicate) {
		Write-Output "Duplicate Located with a cost of $($duplicateCost.Cost), across $($duplicateCost.Fragment.Count) Fragments";

		foreach ($fragment in $duplicateCost.Fragment) {
			Write-Output "File Name: $($fragment.FileName) Line Numbers: $($fragment.LineRange.Start) - $($fragment.LineRange.End)";
			Write-Output "Offending Text: $($fragment.Text)";
		}

    $anyFailures = $TRUE;

		if(isAppVeyor) {
			Add-AppveyorTest "Duplicate Located with a cost of $($duplicateCost.Cost), across $($duplicateCost.Fragment.Count) Fragments" -Outcome Failed -ErrorMessage "See dupFinder.html in build artifacts for full details of duplicates";
		}
	}

	if(isAppVeyor) {
			Push-AppveyorArtifact $dupFinderResultsFile;
	}

	if ($anyFailures -eq $TRUE){
		Write-Output "Failing build as there are duplicates in the Code Base";
		throw "Duplicates found in code base";
	}
}

function analyseInspectCodeResults( [Parameter(ValueFromPipeline=$true)]$inspectCodeResultsFile ) {
  $inspectCodeErrors = [xml](Get-Content $inspectCodeResultsFile);
	$anyFailures = $FALSE;

  foreach ($errorIssueType in $inspectCodeErrors.SelectNodes("/Report/IssueTypes/IssueType[@Severity='ERROR']")) {
    Write-Output "Code Inspection Error(s) Located. Description: $($errorIssueType.Description)";

    $issueLookup = "/Report/Issues/Project/Issue[@TypeId='$($errorIssueType.Id)']";
    foreach ($errorIssue in $inspectCodeErrors.SelectNodes($issueLookup)) {
      Write-Output "File Name: $($errorIssue.File) Line Number: $($errorIssue.Line) Message: $($errorIssue.Message)";

      if(isAppVeyor) {
			  Add-AppveyorTest "Code Inspection Error: $($errorIssueType.Description) Line Number: $($errorIssue.Line)" -Outcome Failed -FileName "$($errorIssue.File)" -ErrorMessage $($errorIssue.Message);
		  }
    }

    $anyFailures = $TRUE;
  }

	if(isAppVeyor) {
			Push-AppveyorArtifact $inspectCodeResultsFile;
	}

	if ($anyFailures -eq $TRUE){
		Write-Output "Failing build as there are code inspection errors in the Code Base";
		throw "Code Inspection Errors found in code base";
	}
}

function testEnvironmentVariable($envVariableName, $envVariableValue) {
	if($envVariableValue -ne $null) {
		Write-Output "$envVariableName : $envVariableValue";
	} else {
		Write-Output "$envVariableName : Not Defined";
	}
}

function applyXslTransform($xmlFile, $xslFile, $outputFile) {
	Write-Output "XML File: $xmlFile"
	Write-Output "XSL File: $xslFile"
	Write-Output "Output File: $outputFile"

	$xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
	$xslt.Load($xslFile);
	$xslt.Transform($xmlFile, $outputFile);

	Write-Output "XSL Transform completed."

	if(isAppVeyor) {
			Push-AppveyorArtifact $outputFile;
	}
}

Task -Name Default -Depends BuildSolution

# private tasks

Task -Name __EchoAppVeyorEnvironmentVariables -Description $private -Action {
	if(isAppVeyor) {
		testEnvironmentVariable "CI" $env:CI;
		testEnvironmentVariable "APPVEYOR_API_URL" $env:APPVEYOR_API_URL;
		testEnvironmentVariable "APPVEYOR_PROJECT_ID" $env:APPVEYOR_PROJECT_ID;
		testEnvironmentVariable "APPVEYOR_PROJECT_NAME" $env:APPVEYOR_PROJECT_NAME;
		testEnvironmentVariable "APPVEYOR_PROJECT_SLUG" $env:APPVEYOR_PROJECT_SLUG;
		testEnvironmentVariable "APPVEYOR_BUILD_FOLDER" $env:APPVEYOR_BUILD_FOLDER;
		testEnvironmentVariable "APPVEYOR_BUILD_ID" $env:APPVEYOR_BUILD_ID;
		testEnvironmentVariable "APPVEYOR_BUILD_NUMBER" $env:APPVEYOR_BUILD_NUMBER;
		testEnvironmentVariable "APPVEYOR_BUILD_VERSION" $env:APPVEYOR_BUILD_VERSION;
		testEnvironmentVariable "APPVEYOR_PULL_REQUEST_NUMBER" $env:APPVEYOR_PULL_REQUEST_NUMBER;
		testEnvironmentVariable "APPVEYOR_PULL_REQUEST_TITLE" $env:APPVEYOR_PULL_REQUEST_TITLE;
		testEnvironmentVariable "APPVEYOR_JOB_ID" $env:APPVEYOR_JOB_ID;
		testEnvironmentVariable "APPVEYOR_REPO_PROVIDER" $env:APPVEYOR_REPO_PROVIDER;
		testEnvironmentVariable "APPVEYOR_REPO_SCM" $env:APPVEYOR_REPO_SCM;
		testEnvironmentVariable "APPVEYOR_REPO_NAME" $env:APPVEYOR_REPO_NAME;
		testEnvironmentVariable "APPVEYOR_REPO_BRANCH" $env:APPVEYOR_REPO_BRANCH;
    testEnvironmentVariable "APPVEYOR_REPO_TAG" $env:APPVEYOR_REPO_TAG;
    testEnvironmentVariable "APPVEYOR_REPO_TAG_NAME" $env:APPVEYOR_REPO_TAG_NAME;
		testEnvironmentVariable "APPVEYOR_REPO_COMMIT" $env:APPVEYOR_REPO_COMMIT;
		testEnvironmentVariable "APPVEYOR_REPO_COMMIT_AUTHOR" $env:APPVEYOR_REPO_COMMIT_AUTHOR;
		testEnvironmentVariable "APPVEYOR_REPO_COMMIT_TIMESTAMP" $env:APPVEYOR_REPO_COMMIT_TIMESTAMP;
		testEnvironmentVariable "APPVEYOR_SCHEDULED_BUILD" $env:APPVEYOR_SCHEDULED_BUILD;
		testEnvironmentVariable "PLATFORM" $env:PLATFORM;
		testEnvironmentVariable "CONFIGURATION" $env:CONFIGURATION;
	}
}

Task -Name __VerifyConfiguration -Description $private -Action {
	Assert ( @('Debug', 'Release') -contains $config ) "Unknown configuration, $config; expecting 'Debug' or 'Release'";
}

Task -Name __CreateBuildArtifactsDirectory -Description $private -Action {
	get-buildArtifactsDirectory | create-packageDirectory;
}

Task -Name __RemoveBuildArtifactsDirectory -Description $private -Action {
	get-buildArtifactsDirectory | remove-packageDirectory;
}

Task -Name __InstallChocolatey -Description $private -Action {
	if(isChocolateyInstalled) {
		Write-Output "Chocolatey already installed";
    Write-Output "Updating to latest Chocolatey..."

    if(Test-Path -Path (Join-Path -Path $script:chocolateyDir -ChildPath "choco.exe")) {
      $script:chocolateyCommand = Join-Path $script:chocolateyDir -ChildPath "choco.exe"
    } else {
      $script:chocolateyCommand = Join-Path (Join-Path $script:chocolateyDir "chocolateyInstall") -ChildPath "chocolatey.cmd";
    }

    exec {
			Invoke-Expression "$script:chocolateyCommand upgrade chocolatey";
		}

    Write-Output "Latest Chocolatey installed."
	}	else {
		try {
			Write-Output "Chocolatey is not installed, installing Chocolatey...";

			exec {
				Invoke-Expression ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'));
			}

			$script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey
			if (-not (Test-Path $script:chocolateyDir)) {
				throw "Error installing Chocolatey"
			}

			Write-Output ("************ Install Chocolatey Successful ************")
		}
		catch {
			Write-Error $_
			Write-Output ("************ Install Chocolatey Failed ************")
		}
	}
}

Task -Name __InstallReSharperCommandLineTools -Depends __InstallChocolatey -Description $private -Action {
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$inspectCodeExe = Join-Path $chocolateyBinDir -ChildPath "inspectcode.exe";

	try {
		Write-Output "Running Install Command Line Tools..."

		if (-not (Test-Path $inspectCodeExe)) {
			exec {
				Invoke-Expression "$script:chocolateyCommand install resharper-clt -y";
			}
		} else {
			Write-Output "resharper-clt already installed";
		}

		Write-Output ("************ Install Command Line Tools Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Install Command Line Tools Failed ************")
	}
}

Task -Name __UpdateReSharperCommandLineTools -Description $private -Action {
	try {
		Write-Output "Running Upgrade Command Line Tools..."

		exec {
			Invoke-Expression "$script:chocolateyCommand upgrade resharper-clt";
		}

		Write-Output ("************ Upgrade Command Line Tools Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Upgrade Command Line Tools Failed ************")
	}
}

Task -Name __InstallPSBuild -Description $private -Action {
	try {
		Write-Output "Running Install PSBuild..."

		exec {
			$psbuildLoaded = $false;
			if(get-command invoke-msbuild -ErrorAction SilentlyContinue) {
				$psbuildLoaded=$true
			}

			if($psbuildLoaded) {
				Write-Output "PSBuild is already installed";
			} else {
      	Write-Output "PSBuild is not already installed";
		(new-object Net.WebClient).DownloadString("https://raw.github.com/ligershark/psbuild/master/src/GetPSBuild.ps1") | Invoke-Expression;
			}
		}

		Write-Output ("************ Install PSBuild Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Install PSBuild Failed ************")
	}
}

Task -Name __InstallGitVersion -Depends __InstallChocolatey -Description $private -Action {
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";

	try {
		Write-Output "Running Install GitVersion.Portable..."

		if (-not (Test-Path $gitVersionExe)) {
			exec {
							Invoke-Expression "$script:chocolateyCommand install GitVersion.Portable -pre -y";
			}
		} else {
			Write-Output "GitVersion.Portable already installed";
		}

		Write-Output ("************ Install GitVersion.Portable Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Install GitVersion.Portable Failed ************")
	}
}

Task -Name __InstallGitReleaseManager -Depends __InstallChocolatey -Description $private -Action {
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$gitReleaseManagerExe = Join-Path $chocolateyBinDir -ChildPath "GitReleaseManager.exe";

	try {
		Write-Output "Running Install GitReleaseManager.Portable..."

		if (-not (Test-Path $gitReleaseManagerExe)) {
			exec {
							Invoke-Expression "$script:chocolateyCommand install GitReleaseManager.Portable -pre -y";
			}
		} else {
			Write-Output "GitReleaseManager.Portable already installed";
		}

		Write-Output ("************ Install GitReleaseManager.Portable Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Install GitReleaseManager.Portable Failed ************")
	}
}

Task -Name __UpdateGitVersion -Description $private -Action {
	try {
		Write-Output "Running Upgrade GitVersion.Portable..."

		exec {
			Invoke-Expression "$script:chocolateyCommand upgrade GitVersion.Portable";
		}

		Write-Output ("************ Upgrade GitVersion.Portable Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Upgrade GitVersion.Portable Failed ************")
	}
}

# primary targets

Task -Name InspectCodeForProblems -Depends RebuildSolution, RunDupFinder, RunInspectCode -Description "Complete build, including running dupfinder, and inspectcode."

Task -Name DeployMasterSolutionToMyGet -Depends InspectCodeForProblems, DeployMasterPackageToMyGet, CreateGitHubReleaseNotes -Description "Complete build, including creation of Chocolatey Package and Deployment to MyGet.org"

Task -Name DeploySolutionToGitHub -Depends ExportGitHubReleaseNotes, InspectCodeForProblems, DeployPackageToChocolatey, AddAssetsToGitHubRelease, CloseMilestone -Description "Complete build, including creation of LMS agent package and Deployment to GitHub releases."

# build tasks

Task -Name RunGitVersion -Depends __InstallGitVersion -Description "Execute the GitVersion Command Line Tool, to figure out what the current semantic version of the repository is" -Action {
	$rootDirectory = get-rootDirectory;
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";

	try {
		Write-Output "Running RunGitVersion..."

		exec {
			if(isAppVeyor) {
				Write-Output "Running on AppVeyor, so UpdateAssemblyInfo will be called."
				& $gitVersionExe /l console /output buildserver /updateAssemblyInfo /nofetch
				$script:version = $env:GitVersion_LegacySemVerPadded
			} else {
				$output = & $gitVersionExe
				$joined = $output -join "`n"
				$versionInfo = $joined | ConvertFrom-Json
				$script:version = $versionInfo.LegacySemVerPadded
			}

			Write-Output "Calculated Legacy SemVer Padded Version Number: $script:version"
		}

		Write-Output ("************ RunGitVersion Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ RunGitVersion Failed ************")
	}
}

Task -Name RunInspectCode -Depends __InstallReSharperCommandLineTools -Description "Execute the InspectCode Command Line Tool" -Action {
  $rootDirectory = get-rootDirectory;
  $buildArtifactsDirectory = get-buildArtifactsDirectory;
	$buildScriptsDirectory = get-buildScriptsDirectory;
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";

	$inspectCodeExe = Join-Path $chocolateyBinDir -ChildPath "inspectcode.exe";
	$inspectCodeConfigFile = Join-Path $buildScriptsDirectory -ChildPath "inspectcode.config";
	$inspectCodeXmlFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "inspectcode.xml";
	$inspectCodeXslFile = Join-Path -Path $buildScriptsDirectory -ChildPath "inspectcode.xsl";
  $inspectCodeHtmlFile = $inspectCodeXmlFile -replace ".xml", ".html";

  (Get-Content -path $inspectCodeConfigFile) | % { $_ -Replace '%RootDirectory%', $rootDirectory } |  Out-File $inspectCodeConfigFile

	exec {
		Invoke-Expression "$inspectCodeExe /config=$inspectCodeConfigFile";

		if(Test-Path $inspectCodeXmlFile) {
      applyXslTransform $inspectCodeXmlFile $inspectCodeXslFile $inspectCodeHtmlFile;
			$inspectCodeXmlFile | analyseInspectCodeResults;
		}

		# Reset the inspectcode.config file
		git checkout $inspectCodeConfigFile;
	}
}

Task -Name RunDupFinder -Depends __InstallReSharperCommandLineTools -Description "Execute the DupFinder Command Line Tool" -Action {
	$rootDirectory = get-rootDirectory;
	$buildArtifactsDirectory = get-buildArtifactsDirectory;
	$buildScriptsDirectory = get-buildScriptsDirectory;
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";

	$dupFinderExe = Join-Path $chocolateyBinDir -ChildPath "dupfinder.exe";
	$dupFinderConfigFile = Join-Path $buildScriptsDirectory -ChildPath "dupfinder.config";
	$dupFinderXmlFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "dupfinder.xml";
	$dupFinderXslFile = Join-Path -Path $buildScriptsDirectory -ChildPath "dupfinder.xsl";
	$dupFinderHtmlFile = $dupFinderXmlFile -replace ".xml", ".html";

	(Get-Content -path $dupFinderConfigFile) | % { $_ -Replace '%RootDirectory%', $rootDirectory } |  Out-File $dupFinderConfigFile

	exec {
		Invoke-Expression "$dupFinderExe /config=$dupFinderConfigFile";

		if(Test-Path $dupFinderXmlFile) {
			applyXslTransform $dupFinderXmlFile $dupFinderXslFile $dupFinderHtmlFile;
      $dupFinderXmlFile | analyseDupFinderResults;
		}

		# Reset the dupfinder.config file
		git checkout $dupFinderConfigFile;
	}
}

Task -Name OutputNugetVersion -Description "So that we are clear which version of NuGet is being used, call NuGet" -Action {
	try {
		Write-Output "Running NuGet..."

		exec {
			& $nugetExe;
		}

		Write-Output ("************ NuGet Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ NuGet Failed ************")
	}
}

Task -Name NugetPackageRestore -Depends OutputNugetVersion -Description "Restores all the required NuGet packages for this solution, before running the build" -Action {
	$sourceDirectory = get-sourceDirectory;

	try {
		Write-Output "Running NugetPackageRestore..."

		exec {
			& $nugetExe restore "$sourceDirectory\LicenseMonitoringSystem.sln"
		}

		Write-Output ("************ NugetPackageRestore Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ NugetPackageRestore Failed ************")
	}
}

Task -Name BuildSolution -Depends __RemoveBuildArtifactsDirectory, __VerifyConfiguration, __InstallPSBuild, __EchoAppVeyorEnvironmentVariables, RunGitVersion, NugetPackageRestore -Description "Builds the main solution for the package" -Action {
	$sourceDirectory = get-sourceDirectory;
	$buildArtifactsDirectory = get-buildArtifactsDirectory;
	$buildScriptsDirectory = get-buildScriptsDirectory;

	try {
		Write-Output "Running BuildSolution..."

		exec {
			Invoke-MSBuild "$env:appveyor_build_folder\src\LicenseMonitoringSystem.sln" -NoLogo -Configuration $config -Platform $platform -Targets Build -DetailedSummary -VisualStudioVersion  14.0;

			if(isAppVeyor) {
				$expectedMsiFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "LmsInstaller.exe"
				if(Test-Path $expectedMsiFile) {
					Push-AppveyorArtifact $expectedMsiFile;
				}
			}
		}

		Write-Output ("************ BuildSolution Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ BuildSolution Failed ************")
	}
}

Task -Name RebuildSolution -Depends CleanSolution, __CreateBuildArtifactsDirectory, BuildSolution -Description "Rebuilds the main solution for the package"

# clean tasks

Task -Name CleanSolution -Depends __InstallPSBuild, __RemoveBuildArtifactsDirectory, __VerifyConfiguration -Description "Deletes all build artifacts" -Action {
	$sourceDirectory = get-sourceDirectory;

	try {
		Write-Output "Running CleanSolution..."

		exec {
			Invoke-MSBuild "$sourceDirectory\LicenseMonitoringSystem.sln" -NoLogo -Configuration $config -Platform $platform -Targets Clean -DetailedSummary;
		}

		Write-Output ("************ CleanSolution Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ CleanSolution Failed ************")
	}
}

# package tasks

Task -Name CreateGitHubReleaseNotes -Depends __InstallGitReleaseManager -Description "Using the generated version number, create a draft release on GitHub" -Action {
	try {
		Write-Output "Creating GitHub Release Notes..."
    $rootDirectory = get-rootDirectory;
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $gitReleaseManagerExe = Join-Path $chocolateyBinDir -ChildPath "GitReleaseManager.exe";

		exec {
			& $gitReleaseManagerExe create -c master -u $env:GitHubUserName -p $env:GitHubPassword -o CentralTechnology -r lms-agent -m $script:version -d $rootDirectory
		}

		Write-Output ("************ Create GitHub Release Notes Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Create GitHub Release Notes Failed ************")
	}
}

Task -Name ExportGitHubReleaseNotes -Depends __InstallGitReleaseManager -Description "Using the Release Notes stored on GitHub, generate a new CHANGELOG.md file" -Action {
	try {
		Write-Output "Exporting GitHub Release Notes..."
    $rootDirectory = get-rootDirectory;
    $changeLogFilePath = Join-Path -Path $rootDirectory -ChildPath "CHANGELOG.md";
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $gitReleaseManagerExe = Join-Path $chocolateyBinDir -ChildPath "GitReleaseManager.exe";

		exec {
			& $gitReleaseManagerExe export -f $changeLogFilePath -u $env:GitHubUserName -p $env:GitHubPassword -o chocolatey -r chocolateygui -d $rootDirectory
		}

		Write-Output ("************ Export GitHub Release Notes Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Export GitHub Release Notes Failed ************")
	}
}

Task -Name AddAssetsToGitHubRelease -Depends __InstallGitReleaseManager -Description "Now that we know all is well, upload msi and Chocolatey Package to release." -Action {
try {
		Write-Output "Adding assets to GitHub Release..."
    $rootDirectory = get-rootDirectory;
    $buildArtifactsDirectory = get-buildArtifactsDirectory;
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $gitReleaseManagerExe = Join-Path $chocolateyBinDir -ChildPath "GitReleaseManager.exe";

		exec {
			Get-ChildItem $buildArtifactsDirectory -Filter *.nupkg | Foreach-Object {
				$nugetPath = ($_ | Resolve-Path).Path;
	      $convertedPath = Convert-Path $nugetPath;

				& $gitReleaseManagerExe addasset -a $convertedPath -t $script:version -u $env:GitHubUserName -p $env:GitHubPassword -o chocolatey -r chocolateygui -d $rootDirectory
			}

			& $gitReleaseManagerExe addasset -a "$buildArtifactsDirectory\LMS.exe" -t $script:version -u $env:GitHubUserName -p $env:GitHubPassword -o CentralTechnology -r lms-agent -d $rootDirectory
		}

		Write-Output ("************ Adding assets Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Adding assets Failed ************")
	}
}

Task -Name CloseMilestone -Depends __InstallGitReleaseManager -Description "Now that all work is done, let's close the milestone associated with this release." -Action {
try {
		Write-Output "Closing GitHub Milestone..."
    $rootDirectory = get-rootDirectory;
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $gitReleaseManagerExe = Join-Path $chocolateyBinDir -ChildPath "GitReleaseManager.exe";

		exec {
			& $gitReleaseManagerExe close -m $script:version -u $env:GitHubUserName -p $env:GitHubPassword -o chocolatey -r chocolateygui d $rootDirectory
		}

		Write-Output ("************ Closing GitHub Milestone Successful ************")
	}
	catch {
		Write-Error $_
		Write-Output ("************ Closing GitHub Milestone Failed ************")
	}
}