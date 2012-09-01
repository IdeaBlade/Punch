param($installPath, $toolsPath, $package, $project)

# The following three constants must be updated to reflect the correct versions
$extensionId = "DF2012_IdeaBladeDesigner,7.0.0.0"
$extensionVsix = "IdeaBlade.VisualStudio.OM.Designer.11.0.vsix"
$templatesVsix = "IdeaBlade.VisualStudio.TemplateInstaller.vsix"

$registryRoot = $DTE.RegistryRoot
$extensionsTypesPath = "hkcu:\$registryRoot\ExtensionManager\ExtensionTypes"

if ((Test-Path $extensionsTypesPath))
{
    $extensionTypes = Get-Item -Path $extensionsTypesPath
}

if (!$extensionTypes -or !$extensionTypes.GetValue($extensionId))
{
    $parts = $DTE.FileName.split("\")
    $parts[$parts.Count - 1] = "VSIXInstaller.exe"

    $installer = [string]::Join("\", $parts) 
    $extensionVsixPath = $toolsPath + "\$extensionVsix"
    $templatesVsixPath = $toolsPath + "\$templatesVsix"

    $process = [diagnostics.process]::Start($installer, "`"$extensionVsixPath`"")
    $process.WaitForExit()
    $process = [diagnostics.process]::Start($installer, "`"$templatesVsixPath`"")
    $process.WaitForExit()

    [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | Out-Null
    [Windows.Forms.MessageBox]::Show("You must restart Microsoft Visual Studio in order for the changes to take effect.", 
        "Extensions and Updates", [Windows.Forms.MessageBoxButtons]::OK, 
        [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null

}
else
{
    { "DevForce 2012 EDMX Extension is already installed." }
}