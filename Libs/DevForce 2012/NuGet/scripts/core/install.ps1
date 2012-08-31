param($installPath, $toolsPath, $package, $project)

$project = Get-Project   # for testing

# "Install" IdeaBladeConfig.xsd if it doesn't exist
[Array] $parts = $project.DTE.FileName.split("\")
$xmlSchema = [string]::Join("\", $parts[0..($parts.Count - 4)]) + "\Xml\Schemas\IdeaBladeConfig.xsd"

if (!(Test-Path $xmlSchema))
{
    { "Schema file does not exist" }
}

