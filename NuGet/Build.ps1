# Central repository path
$repository = "\\ibshare\NuGet"

foreach ($nuspecFile in Get-ChildItem ./*.nuspec)
{
    [xml] $spec = Get-Content -Path $nuspecFile.FullName
    [string] $version = $spec.package.metadata.version
    
    $packageName = [System.IO.Path]::GetFileNameWithoutExtension($nuspecFile.Name)
    $packageFileName = "$packageName.$version.nupkg"
    $packageFullName = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($nuspecFile.FullName), $packageFileName)
    
    # Build package
    Write-Host "Building package $packageFileName" -foregroundcolor green
    nuget pack $nuspecFile.FullName -Symbols
    
    if (Test-Path $repository)
    {
        Copy-Item -Path $packageFullName -Destination $repository
    }
    else
    {
        Write-Host "Warning: Cannot copy $packageFullName to $repository. Target path does not exist!" -foregroundcolor yellow
    }
    
    Write-Host ""
}
