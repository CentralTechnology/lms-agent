param([string]$ProjectDir,
	[string]$Configuration,
	[string]$OutDir
);

$ConfigurationDir = "C:\temp";
$ConfigurationFile = "C:\temp\configuration.json"

New-Item -ItemType Directory -Path $ConfigurationDir -Force

$Config = @{}

if([System.IO.File]::Exists($ConfigurationFile)){

	Write-Verbose -Message "Existing configuration found! Loading into memory.";

	$FileContent = Get-Content -Path $ConfigurationFile -Raw
	(ConvertFrom-Json $FileContent).PSObject.Properties | Foreach { $Config[$_.Name] = $_.Value }

	Write-Verbose -Message ($Config | Format-Table -AutoSize | Out-String)

	$Config["ProjectDir"] = "$ProjectDir"
	$Config["OutDir"] = "$OutDir"
	$Config["Configuration"] = "$Configuration"

}else{

$Config = @{ "ProjectDir" = "$ProjectDir"; "Configuration" = "$Configuration"; "OutDir" = "$OutDir" }

}

Write-Verbose -Message ($Config | Format-Table -AutoSize | Out-String)

$Config | ConvertTo-Json | Out-File $ConfigurationFile 

Write-Verbose -Message "Configuration written back to disk!"

Exit 0