$source = "..\*\*\*\*.nupkg"
$destination = "C:\nuget"

Copy-Item -Force -Recurse -Verbose $source -Destination $destination