param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

# 基準となるディレクトリを取得
$ProjectRoot = $PSScriptRoot
if (-not $ProjectRoot) { $ProjectRoot = Get-Location }

# まずReactorのリリースビルドを実行
Write-Host "--- Starting Reactor Release Build ---"
& "$ProjectRoot\build-reactor.ps1" -Configuration "Release"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Reactor build failed. Aborting Submerged build."
    exit 1
}

Write-Host "--- Starting Submerged $Configuration Build ---"

# プロジェクトファイルのパス
$SubmergedProj = "$ProjectRoot\Submerged\Submerged\Submerged.csproj"

if (-not (Test-Path $SubmergedProj)) {
    Write-Host "Error: Cannot find $SubmergedProj"
    exit 1
}

# ビルド実行 (プロジェクトファイルを直接指定)
Write-Host "Building Submerged: $SubmergedProj"
dotnet build "$SubmergedProj" -c $Configuration --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    $sourcePath = "$ProjectRoot\Submerged\Submerged\bin\$Configuration\net6.0\Submerged.dll"
    $destinationPath = "$ProjectRoot\SDK\"

    if (Test-Path $sourcePath) {
        Copy-Item -Path $sourcePath -Destination $destinationPath -Force
        Write-Host "Submerged.dll copied to RebuildUs SDK folder."
    }
    else {
        Write-Host "Error: Build succeeded but DLL not found at $sourcePath"
    }
}
else {
    Write-Host "Submerged build failed."
}

