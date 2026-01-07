$ErrorActionPreference = 'Stop'
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Host "Building CosmeticsDownloader (C#)..." -ForegroundColor Cyan

$projectPath = "$scriptPath\CosmeticsDownloader\CosmeticsDownloader.csproj"

# Build for Debug
$debugOut = "$scriptPath\RebuildUs\bin\Debug\net6.0"
dotnet publish $projectPath -c Debug -o $debugOut -r win-x64 --self-contained false /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfContained=true
if ($LASTEXITCODE -ne 0) {
    Write-Error "C# build (Debug) failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

# Build for Release
$releaseOut = "$scriptPath\RebuildUs\bin\Release\net6.0"
dotnet publish $projectPath -c Release -o $releaseOut -r win-x64 --self-contained false /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfContained=true
if ($LASTEXITCODE -ne 0) {
    Write-Error "C# build (Release) failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "CosmeticsDownloader built successfully." -ForegroundColor Green
