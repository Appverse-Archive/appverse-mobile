param($installPath, $toolsPath, $package, $project)

# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

#save the project file first - this commits the changes made by NuGet before this script runs.
$project.Save()

# Update file date in order to trigger Visual Studio to reload project.
Set-ItemProperty -Path $project.FullName -Name LastWriteTime -Value (get-date)

if ($dte -eq $null -or $dte.Solution -eq $null)
{
	return
}

$defaultPlatform = "ARM"

$activeContext = $dte.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts | Select -First 1 
if ($activeContext -eq $null -or $activeContext.PlatformName -eq $defaultPlatform)
{
	return
}

# Try find the configuration matching the default platform and the currently selected configuration, e.g. Debug or Release.
$defaultConfiguration = $dte.Solution.SolutionBuild.SolutionConfigurations | 
						%{ $_.SolutionContexts } | 
						? { $_.PlatformName -eq $defaultPlatform -and $_.ConfigurationName -eq $activeContext.ConfigurationName } | 
						Select -First 1
if ($defaultConfiguration -ne $null)
{
	$defaultConfiguration.Collection.Parent.Activate();
}