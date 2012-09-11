param($installPath, $toolsPath, $package, $project)

#$project = Get-Project   # for testing

# Remove DevForce build tasks
$buildProject = $project | Get-MSBuildProject

$buildProject.Xml.Imports | 
    Where-Object { $_.Project.ToLowerInvariant().EndsWith("ideablade.devforce.common.targets") } | 
    ForEach-Object { $buildProject.Xml.RemoveChild($_) }
