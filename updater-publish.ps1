$ErrorActionPreference = 'Stop'
go build -o BepInExUpdater\publish\BepInExUpdater.exe BepInExUpdater\main.go
if ($LASTEXITCODE -ne 0) {
    Write-Error "Go build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}