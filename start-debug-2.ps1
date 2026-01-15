$ErrorActionPreference = 'Stop'

& "$PSScriptRoot\taskkill.ps1"

Write-Host "Building project..." -ForegroundColor Cyan
& "$PSScriptRoot\build-debug.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

$gamePath = Get-Content -Path "$PSScriptRoot\debugenv.txt" -Raw
$gamePath = $gamePath.Trim()

$dllSource = "$PSScriptRoot\RebuildUs\bin\Debug\net6.0\RebuildUs.dll"
$pluginDir = Join-Path $gamePath "BepInEx\plugins"
$dllDest = Join-Path $pluginDir "RebuildUs.dll"

if (-not (Test-Path $pluginDir)) {
    New-Item -ItemType Directory -Path $pluginDir -Force
}

if (Test-Path $dllSource) {
    Write-Host "Copying $dllSource to $dllDest..."
    Copy-Item -Path $dllSource -Destination $dllDest -Force
}
else {
    Write-Error "Source DLL not found: $dllSource. Please build the project first."
    exit 1
}

$downloaderSource = "$PSScriptRoot\RebuildUs\bin\Debug\net6.0\CosmeticsDownloader.exe"
$downloaderDest = Join-Path $pluginDir "CosmeticsDownloader.exe"
if (Test-Path $downloaderSource) {
    Write-Host "Copying $downloaderSource to $downloaderDest..."
    Copy-Item -Path $downloaderSource -Destination $downloaderDest -Force
}

$exePath = Join-Path $gamePath "Among Us.exe"

if (Test-Path $exePath) {
    Write-Host "Starting Among Us 2 times..."
    for ($i = 1; $i -le 2; $i++) {
        Start-Process -FilePath $exePath
        Start-Sleep -Milliseconds 1000
    }
}
else {
    Write-Error "Among Us.exe not found at $exePath"
    exit 1
}
