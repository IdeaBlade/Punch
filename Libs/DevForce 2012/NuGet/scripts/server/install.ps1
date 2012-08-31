param($installPath, $toolsPath, $package, $project)

#$project = Get-Project   # for testing

if ($project.ExtenderNames.Contains("WebApplication"))
{
    #if this is a web application, add IdeaBlade.EntityModel.Web.dll reference
    $assembly = $installPath + "\lib\net45\IdeaBlade.EntityModel.Web.dll"
    $project.Object.References.Add($assembly)
}
else
{
    #if this is not a web application, remove Global.asax
    $project.ProjectItems | Where-Object { $_.Name -eq 'Global.asax' } | ForEach-Object { $_.Remove() }
}
