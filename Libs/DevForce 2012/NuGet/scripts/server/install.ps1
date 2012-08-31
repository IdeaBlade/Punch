param($installPath, $toolsPath, $package, $project)

#$project = Get-Project   # for testing

#if this is a web application, add IdeaBlade.EntityModel.Web.dll reference
if ($project.ExtenderNames.Contains("WebApplication"))
{
    $assembly = $installPath + "\lib\net45\IdeaBlade.EntityModel.Web.dll"
    $project.Object.References.Add($assembly)
}
