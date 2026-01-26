param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

Set-Location -Path ".\Reactor"

dotnet cake build.cake --target=Build --configuration=$Configuration

if ($LASTEXITCODE -eq 0) {
    $sourcePath = "Reactor\bin\$Configuration\net6.0\Reactor.dll"
    $destinationPath = "..\SDK\"
    Copy-Item -Path $sourcePath -Destination $destinationPath -Force
    Write-Host "Reactor.dll copied to RebuildUs SDK folder."
}
else {
    Write-Host "Build failed. Skipping copy."
}