# RebuildUs.Launcher Build Script

$ProjectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFile = Join-Path $ProjectDir "RebuildUs.Launcher\RebuildUs.Launcher.csproj"
$OutputDirectory = Join-Path $ProjectDir "Release\Launcher"

Write-Host "Building RebuildUs.Launcher..." -ForegroundColor Cyan

# Ensure Output directory exists
if (-not (Test-Path $OutputDirectory)) {
    New-Item -Path $OutputDirectory -ItemType Directory | Out-Null
}

# Build the project
dotnet publish $ProjectFile -c Release -o $OutputDirectory --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Output: $OutputDirectory" -ForegroundColor Green
}
else {
    Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
