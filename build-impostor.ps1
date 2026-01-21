$serverPath = Get-Content (Join-Path $PSScriptRoot "serverenv.txt")
$buildPath = Join-Path $PSScriptRoot "RebuildUs.Impostor\bin\Debug\net8.0"
$pluginsPath = Join-Path $serverPath "plugins"

# Build project
dotnet build (Join-Path $PSScriptRoot "RebuildUs.Impostor\RebuildUs.Impostor.csproj") -c Debug

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

# Ensure plugins directory exists
if (-not (Test-Path $pluginsPath)) {
    New-Item -ItemType Directory -Path $pluginsPath
}

# Copy files
Get-ChildItem -Path $buildPath -Filter *.dll | ForEach-Object {
    if ($_.Name -eq "RebuildUs.Impostor.dll" -or $_.Name -like "Discord.*.dll" -or $_.Name -eq "Newtonsoft.Json.dll") {
        Write-Host "Copying $($_.Name) to plugins folder"
        Copy-Item $_.FullName -Destination $pluginsPath -Force
    }
    else {
        Write-Host "Copying $($_.Name) to server root"
        Copy-Item $_.FullName -Destination $serverPath -Force
    }
}

Write-Host "RebuildUs.Impostor build and copy completed."
