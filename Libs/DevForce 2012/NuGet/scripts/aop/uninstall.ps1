param($installPath, $toolsPath, $package, $project)

$project = Get-Project   # for testing

# Remove DevForce build tasks
$buildProject = $project | Get-MSBuildProject

$buildProject.Xml.Imports | 
    Where-Object { $_.Project.Contains("IdeaBlade") } | 
    ForEach-Object { $buildProject.Xml.RemoveChild($_) }
