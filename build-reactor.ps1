param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

Push-Location
Set-Location -Path ".\Reactor"

Write-Host "Starting Reactor build..." -ForegroundColor Cyan
dotnet cake build.cake --target=Build --configuration=$Configuration

Write-Host "LASTEXITCODE after dotnet cake: $LASTEXITCODE" -ForegroundColor Magenta

if ($LASTEXITCODE -eq 0) {
    $sourcePath = "Reactor\bin\$Configuration\net6.0\Reactor.dll"
    $sourcePath2 = "Reactor\bin\$Configuration\net6.0\Reactor.xml"
    $destinationPath = "..\SDK\"
    Copy-Item -Path $sourcePath -Destination $destinationPath -Force
    Copy-Item -Path $sourcePath2 -Destination $destinationPath -Force
    Write-Host "Reactor.dll copied to RebuildUs SDK folder."
}
else {
    Write-Host "Build failed. Skipping copy."
}

# 呼び出し元に戻る (相対パスではなく絶対パスを使用)
Pop-Location
