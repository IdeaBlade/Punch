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
    $VSPath = [System.IO.Path]::GetDirectoryName($DTE.FileName)

    $installer = [System.IO.Path]::Combine($VSPath, "VSIXInstaller.exe") 
    $extensionVsixPath = [System.IO.Path]::Combine($toolsPath, $extensionVsix)
    $templatesVsixPath = [System.IO.Path]::Combine($toolsPath, $templatesVsix)

    $process = [System.Diagnostics.Process]::Start($installer, "`"$extensionVsixPath`"")
    $process.WaitForExit()
    $process = [System.Diagnostics.Process]::Start($installer, "`"$templatesVsixPath`"")
    $process.WaitForExit()

    [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | Out-Null
    [Windows.Forms.MessageBox]::Show("You must restart Microsoft Visual Studio in order for the changes to take effect.", 
        "Extensions and Updates", [Windows.Forms.MessageBoxButtons]::OK, 
        [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null

    $toolsSetup = [System.IO.Path]::Combine($toolsPath, "setup.exe")
    [System.Diagnostics.Process]::Start($toolsSetup)
}
else
{
    { "DevForce 2012 EDMX Extension is already installed." }
}