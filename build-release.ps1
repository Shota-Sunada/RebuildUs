$AmongUs = $env:AMONG_US

$PluginsFolder = $AmongUs + "\BepInEx\plugins"

$PluginDllFolder = ".\RebuildUs\bin\Release\net6.0"
$PluginDllFile = $PluginDllFolder + "\RebuildUs.dll"
$ProjectFile = ".\RebuildUs\RebuildUs.csproj"

Write-Host "PowerShell $( $PSVersionTable.PSEdition ) Version $( $PSVersionTable.PSVersion )" -ForegroundColor Cyan
Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"; trap
{
    Write-Error $_ -ErrorAction Continue; exit 1
}

function Exec([scriptblock] $cmd)
{
    & $cmd
    if ($LASTEXITCODE)
    {
        exit $LASTEXITCODE
    }
}

# If dotnet CLI is installed globally and it matches requested version, use for execution
if ($null -ne (Get-Command "dotnet" -ErrorAction SilentlyContinue) -and   `
      $( dotnet --version ) -and $LASTEXITCODE -eq 0)
{
    $env:DOTNET_EXE = (Get-Command "dotnet").Path
}
else
{
    # Download install script
    $DotNetInstallFile = "$TempDirectory\dotnet-install.ps1"
    New-Item -ItemType Directory -Path $TempDirectory -Force | Out-Null
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    (New-Object System.Net.WebClient).DownloadFile($DotNetInstallUrl, $DotNetInstallFile)

    # If global.json exists, load expected version
    if (Test-Path $DotNetGlobalFile)
    {
        $DotNetGlobal = $( Get-Content $DotNetGlobalFile | Out-String | ConvertFrom-Json )
        if ($DotNetGlobal.PSObject.Properties["sdk"] -and $DotNetGlobal.sdk.PSObject.Properties["version"])
        {
            $DotNetVersion = $DotNetGlobal.sdk.version
        }
    }

    # Install by channel or version
    $DotNetDirectory = "$TempDirectory\dotnet-win"
    if (!(Test-Path variable:DotNetVersion))
    {
        ExecSafe { & powershell $DotNetInstallFile -InstallDir $DotNetDirectory -Channel $DotNetChannel -NoPath }
    }
    else
    {
        ExecSafe { & powershell $DotNetInstallFile -InstallDir $DotNetDirectory -Version $DotNetVersion -NoPath }
    }
    $env:DOTNET_EXE = "$DotNetDirectory\dotnet.exe"
}

Write-Host "Microsoft (R) .NET Core SDK Version $( & $env:DOTNET_EXE --version )"
Write-Host "Starting Submerged Release Build..." -ForegroundColor Yellow
& "$PSScriptRoot\build-submerged.ps1" -Configuration "Release"
Write-Host "Submerged Release Build completed." -ForegroundColor Green
# Sync version from Main.cs to csproj
$ModVersion = (Get-Content ".\RebuildUs\Main.cs" | Select-String 'public const string MOD_VERSION = "([^"]+)"').Matches.Groups[1].Value
$CsprojContent = Get-Content $ProjectFile
$CsprojContent = $CsprojContent -replace '<Version>[^<]+</Version>', "<Version>$ModVersion</Version>"
$CsprojContent | Set-Content $ProjectFile
Write-Host "[Build] In progress..." -ForegroundColor DarkBlue
Exec { & $env:DOTNET_EXE build $ProjectFile -c Release /nodeReuse:false /p:UseSharedCompilation=false -nologo -clp:NoSummary --verbosity quiet }

Write-Host "[Build] Done!" -ForegroundColor Green

$PluginVersion = (Get-Command $PluginDllFile).FileVersionInfo.FileVersion

Write-Host "[Copy] In progress..." -ForegroundColor DarkBlue
if (!(Test-Path $PluginsFolder))
{
    New-Item -ItemType Directory -Path $PluginsFolder -Force | Out-Null
}
try {
    Copy-Item $PluginDllFile $PluginsFolder
    Write-Host "Copied to PluginsFolder successfully." -ForegroundColor Green
} catch {
    Write-Host "Failed to copy to PluginsFolder, skipping." -ForegroundColor Yellow
}
Copy-Item ".\SDK\Submerged.dll" $PluginsFolder
Copy-Item ".\SDK\Reactor.dll" $PluginsFolder
Write-Host "[Copy] Done!" -ForegroundColor Green

Write-Host "Plugin Version is $( $PluginVersion )" -ForegroundColor Magenta
Write-Host "All process was successfully done!" -ForegroundColor Green
