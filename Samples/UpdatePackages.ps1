foreach ($solution in dir -Path .\ -Filter "*.sln" -Recurse)
{
    $solutionDir = [IO.Path]::GetDirectoryName($solution.FullName)
    $packages = [IO.Path]::Combine($solutionDir, "packages")

    # Restore packages
    foreach ($pkg in dir -Path $solutionDir -Filter "packages.config" -Recurse)
    {
        nuget install $pkg.FullName -OutputDirectory $packages
    }

    # Update packages
    nuget update $solution.FullName -Id IdeaBlade.Cocktail
    nuget update $solution.FullName -Id IdeaBlade.Cocktail.Utils
    nuget update $solution.FullName -Id IdeaBlade.DevForce.Core
    nuget update $solution.FullName -Id IdeaBlade.DevForce.Aop
    nuget update $solution.FullName -Id IdeaBlade.DevForce.Server
    #nuget update $solution.FullName -Id PostSharp

    # Delete packages folder. It will be restored on next build
    if (Test-Path -Path $packages)
    {
        del $packages -Recurse
    }
}