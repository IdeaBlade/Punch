param($installPath, $toolsPath, $package, $project)

#$project = Get-Project   # for testing

# Add DevForce Code-First build tasks
Add-Import "$toolsPath\IdeaBlade.DevForce.Common.targets" $project.ProjectName
