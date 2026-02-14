[Setup]
AppName=RebuildUs Launcher
AppVersion=1.0.0
DefaultDirName={autopf}\RebuildUs Launcher
DefaultGroupName=RebuildUs Launcher
UninstallDisplayIcon={app}\RebuildUs.Launcher.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Release\Installer
OutputBaseFilename=RebuildUsLauncherSetup
ArchitecturesInstallIn64BitMode=x64compatible

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "Release\Launcher\RebuildUs.Launcher.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\RebuildUs Launcher"; Filename: "{app}\RebuildUs.Launcher.exe"
Name: "{autodesktop}\RebuildUs Launcher"; Filename: "{app}\RebuildUs.Launcher.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\RebuildUs.Launcher.exe"; Description: "{cm:LaunchProgram,RebuildUs Launcher}"; Flags: nowait postinstall skipifsilent
