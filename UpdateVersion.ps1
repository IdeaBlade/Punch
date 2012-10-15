param($version)

if (!$version)
{
    Write-Host "Error: Must specify a version!" -foregroundcolor red
}
else
{
    $assemblyVersion = $version.Split("-")[0] # Remove pre-release part

    # Update version in AssemblyInfo.cs
    foreach ($file in dir -Path .\ -Filter "AssemblyInfo.cs" -Recurse | ? { !($_.FullName -match "\\Samples\\") })
    {
        $content = [IO.File]::ReadAllText($file.FullName)
        $content = $content -replace "\[assembly: AssemblyVersion\("".*?""\)\]", "[assembly: AssemblyVersion(""$assemblyVersion"")]"
        $content = $content -replace "\[assembly: AssemblyFileVersion\("".*?""\)\]", "[assembly: AssemblyFileVersion(""$assemblyVersion"")]"
        
        [IO.File]::WriteAllText($file.FullName, $content)
    }
    
    # Update version in NuGet packages
    foreach ($file in dir -Path ./NuGet/*.nuspec)
    {
        [xml] $nuspec = Get-Content -Path $file.FullName
        
        # Update package version
        $nuspec.package.metadata.version = $version
        
        # Update dependencies
        foreach ($d in $nuspec.package.metadata.dependencies.dependency | ? { $_.id -match "IdeaBlade.Cocktail" -and $_.version})
        {
            $d.version = "[$version]"
        }
        
        $nuspec.Save($file.FullName)
    }
}
