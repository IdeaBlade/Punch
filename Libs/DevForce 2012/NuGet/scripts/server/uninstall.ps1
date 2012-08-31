param($installPath, $toolsPath, $package, $project)

$project.Object.References | Where-Object { $_.Name -eq 'IdeaBlade.EntityModel.Web' } | ForEach-Object { $_.Remove() }