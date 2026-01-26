param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

Set-Location -Path ".\Reactor"

dotnet cake build.cake --target=Build --configuration=$Configuration