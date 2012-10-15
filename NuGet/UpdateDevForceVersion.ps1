param($version)

if (!$version)
{
    Write-Host "Error: Must specify a version!" -foregroundcolor red
}
else
{
    foreach ($file in dir -Path ./*.nuspec)
    {
        [xml] $nuspec = Get-Content -Path $file.FullName
        
        # Update dependencies
        foreach ($d in $nuspec.package.metadata.dependencies.dependency | ? { $_.id -match "IdeaBlade.DevForce" -and $_.version})
        {
            $d.version = "[$version]"
        }
        
        $nuspec.Save($file.FullName)
    }
}
