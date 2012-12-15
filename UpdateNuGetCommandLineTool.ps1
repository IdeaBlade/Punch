foreach ($file in dir -Path .\ -Filter "nuget.exe" -Recurse)
{
   $process = [System.Diagnostics.Process]::Start($file.FullName, "update -self")
   $process.WaitForExit()
}