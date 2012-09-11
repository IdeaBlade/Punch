param($installPath, $toolsPath, $package, $project)

#$project = Get-Project   # for testing

$targetsFile = [System.IO.Path]::Combine($toolsPath, "IdeaBlade.DevForce.Common.targets")

# Make the path to the targets file relative
$projectUri = New-Object Uri("file://" + $project.FullName)
$targetUri = New-Object Uri("file://" + $targetsFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

# Remove previous imports to IdeaBlade.DevForce.Common.targets
$buildProject = $project | Get-MSBuildProject
$buildProject.Xml.Imports | 
    Where-Object { $_.Project.ToLowerInvariant().EndsWith("ideablade.devforce.common.targets") } | 
    ForEach-Object { $buildProject.Xml.RemoveChild($_) }

# Add import to IdeaBlade.DevForce.Common.targets
$import = Add-Import $relativePath $project.ProjectName
$import.Condition = "Exists('$relativePath')"
