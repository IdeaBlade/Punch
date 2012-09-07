param($installPath, $toolsPath, $package, $project)

#$project = Get-Project   # for testing

# Add DevForce Code-First build tasks
$solutionDir = Get-SolutionDir
$projectRelative = $project.FullName.SubString($solutionDir.Length)
$prefix = ""
for ($i = 0; $i -lt ($projectRelative.Split("\").Count - 2); $i++) { $prefix = $prefix + "..\" }

$toolsRelative = $toolsPath.Replace($solutionDir + "\", $prefix)

$targets = "$toolsRelative\IdeaBlade.DevForce.Common.targets"
$import = Add-Import $targets $project.ProjectName
$import.Condition = "Exists('$targets')"
