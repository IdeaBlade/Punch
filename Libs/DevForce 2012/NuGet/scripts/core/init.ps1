param($installPath, $toolsPath, $package, $project)

#Get the first project
$project = ([Array] (Get-Project -All))[0]

#The following three constants must be updated to reflect the correct versions
$extensionId = "DF2012_IdeaBladeDesigner,7.0.0.0"
$extensionVsix = "IdeaBlade.VisualStudio.OM.Designer.11.0.vsix"
$templatesVsix = "IdeaBlade.VisualStudio.TemplateInstaller.vsix"

$registryRoot = $project.DTE.RegistryRoot
$extensionsTypesPath = "hkcu:\$registryRoot\ExtensionManager\ExtensionTypes"

if ((Test-Path $extensionsTypesPath))
{
    $extensionTypes = Get-Item -Path $extensionsTypesPath
}

if (!$extensionTypes -or !$extensionTypes.GetValue($extensionId))
{
    $parts = $project.DTE.FileName.split("\")
    $parts[$parts.Count - 1] = "VSIXInstaller.exe"

    $installer = [string]::Join("\", $parts) 
    $extensionVsixPath = $toolsPath + "\$extensionVsix"
    $templatesVsixPath = $toolsPath + "\$templatesVsix"

    $process = [diagnostics.process]::Start($installer, "`"$extensionVsixPath`"")
    $process.WaitForExit()
    $process = [diagnostics.process]::Start($installer, "`"$templatesVsixPath`"")
    $process.WaitForExit()
}
else
{
    { "DevForce 2012 EDM Designer Extension is already installed" }
}