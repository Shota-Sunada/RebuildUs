$ErrorActionPreference = 'Stop'

& "$PSScriptRoot\taskkill.ps1"

$gamePath = Get-Content -Path "$PSScriptRoot\debugenv.txt" -Raw
$gamePath = $gamePath.Trim()

$exePath = Join-Path $gamePath "Among Us.exe"

if (Test-Path $exePath)
{
    Write-Host "Starting Among Us"
    Start-Process -FilePath $exePath
}
else
{
    Write-Error "Among Us.exe not found at $exePath"
    exit 1
}