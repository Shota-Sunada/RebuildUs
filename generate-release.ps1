param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition
$mainCsPath = "$scriptPath\RebuildUs\Main.cs"

Write-Host "Updating version to $Version in Main.cs..." -ForegroundColor Cyan
$content = Get-Content $mainCsPath -Raw
$content = $content -replace 'public const string MOD_VERSION = ".*?";', "public const string MOD_VERSION = `"$Version`";"
Set-Content $mainCsPath $content -NoNewline

Write-Host "Closing Among Us..." -ForegroundColor Cyan
Get-Process "Among Us" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Running Release build..." -ForegroundColor Cyan
& "$scriptPath\build-release.ps1"
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Release build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Publishing Updaters..." -ForegroundColor Cyan
& "$scriptPath\updater-publish.ps1"
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Updater publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Generating Release Zip..." -ForegroundColor Cyan
Push-Location "$scriptPath\ReleaseZipGenerator"
try
{
    go run main.go $Version
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "Release Zip generation failed with exit code $LASTEXITCODE"
        exit $LASTEXITCODE
    }
}
finally
{
    Pop-Location
}