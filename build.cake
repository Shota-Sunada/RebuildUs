var target = Argument("target", "BuildDebug");
var configuration = Argument("configuration", "Debug");
var version = Argument("version", string.Empty);
var launchCount = Argument("launchCount", 1);

var root = MakeAbsolute(Directory("./"));
var sdkDir = root.Combine("SDK");
var rebuildUsProject = root.CombineWithFilePath("RebuildUs/RebuildUs.csproj");
var launcherProject = root.CombineWithFilePath("RebuildUs.Launcher/RebuildUs.Launcher.csproj");
var submergedProject = root.CombineWithFilePath("Submerged/Submerged/Submerged.csproj");
var mainCsFile = root.CombineWithFilePath("RebuildUs/Main.cs");
var debugEnvFile = root.CombineWithFilePath("debugenv.txt");
var serverEnvFile = root.CombineWithFilePath("serverenv.txt");
var launcherIssFile = root.CombineWithFilePath("RebuildUs.Launcher.iss");

int RunProcessOrFail(string fileName, string arguments, DirectoryPath workingDirectory = null)
{
    var settings = new ProcessSettings
    {
        Arguments = arguments,
    };

    if (workingDirectory != null)
    {
        settings.WorkingDirectory = workingDirectory;
    }

    var exitCode = StartProcess(fileName, settings);
    if (exitCode != 0)
    {
        throw new Exception(fileName + " failed with exit code " + exitCode + ". args=" + arguments);
    }

    return exitCode;
}

int RunProcess(string fileName, string arguments, DirectoryPath workingDirectory = null)
{
    var settings = new ProcessSettings
    {
        Arguments = arguments,
    };

    if (workingDirectory != null)
    {
        settings.WorkingDirectory = workingDirectory;
    }

    return StartProcess(fileName, settings);
}

string ReadTrimmedTextFile(FilePath path)
{
    var absolutePath = MakeAbsolute(path).FullPath;
    if (!System.IO.File.Exists(absolutePath))
    {
        throw new Exception("Required file was not found: " + absolutePath);
    }

    var text = System.IO.File.ReadAllText(absolutePath).Trim();
    if (string.IsNullOrWhiteSpace(text))
    {
        throw new Exception("Required file is empty: " + absolutePath);
    }

    return text;
}

string GetAmongUsPath()
{
    var amongUsPath = EnvironmentVariable("AMONG_US");
    if (string.IsNullOrWhiteSpace(amongUsPath))
    {
        throw new Exception("AMONG_US environment variable is not set.");
    }

    return amongUsPath;
}

void CopyFileForceToDirectory(FilePath source, DirectoryPath destinationDirectory)
{
    EnsureDirectoryExists(destinationDirectory);
    var destination = destinationDirectory.CombineWithFilePath(source.GetFilename());
    if (FileExists(destination))
    {
        DeleteFile(destination);
    }

    CopyFile(source, destination);
}

void BuildRebuildUs(string config)
{
    Information("Building RebuildUs ({0})...", config);
    DotNetBuild(rebuildUsProject.FullPath, new DotNetBuildSettings
    {
        Configuration = config,
        NoLogo = true,
        Verbosity = DotNetVerbosity.Quiet,
        MSBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("nodeReuse", "false")
            .WithProperty("UseSharedCompilation", "false")
    });
}

void BuildReactorReleaseAndCopyToSdk()
{
    Information("Building Reactor (Release)...");
    RunProcessOrFail("dotnet", "cake build.cake --target=Build --configuration=Release", root.Combine("Reactor"));

    EnsureDirectoryExists(sdkDir);

    var reactorDll = root.CombineWithFilePath("Reactor/Reactor/bin/Release/net6.0/Reactor.dll");
    var reactorXml = root.CombineWithFilePath("Reactor/Reactor/bin/Release/net6.0/Reactor.xml");

    if (!FileExists(reactorDll) || !FileExists(reactorXml))
    {
        throw new Exception("Reactor artifacts were not found after build.");
    }

    CopyFileForceToDirectory(reactorDll, sdkDir);
    CopyFileForceToDirectory(reactorXml, sdkDir);
}

void BuildSubmergedAndCopyToSdk(string config)
{
    Information("Building Submerged ({0})...", config);
    if (!FileExists(submergedProject))
    {
        throw new Exception("Submerged project was not found: " + submergedProject.FullPath);
    }

    RunProcessOrFail("dotnet", "build \"" + submergedProject.FullPath + "\" -c " + config + " --verbosity minimal", root);

    EnsureDirectoryExists(sdkDir);

    var submergedDll = root.CombineWithFilePath("Submerged/Submerged/bin/" + config + "/net6.0/Submerged.dll");
    if (!FileExists(submergedDll))
    {
        throw new Exception("Submerged.dll was not found: " + submergedDll.FullPath);
    }

    CopyFileForceToDirectory(submergedDll, sdkDir);
}

void SyncVersionFromMainToCsproj()
{
    var mainCsPath = MakeAbsolute(mainCsFile).FullPath;
    var csprojPath = MakeAbsolute(rebuildUsProject).FullPath;

    var mainContent = System.IO.File.ReadAllText(mainCsPath);
    var versionMatch = System.Text.RegularExpressions.Regex.Match(mainContent, "public const string MOD_VERSION = \"([^\"]+)\";");
    if (!versionMatch.Success)
    {
        throw new Exception("Could not parse MOD_VERSION from Main.cs.");
    }

    var modVersion = versionMatch.Groups[1].Value;
    var csprojContent = System.IO.File.ReadAllText(csprojPath);
    var replaced = System.Text.RegularExpressions.Regex.Replace(csprojContent, "<Version>[^<]+</Version>", "<Version>" + modVersion + "</Version>");

    System.IO.File.WriteAllText(csprojPath, replaced);
    Information("Synced RebuildUs.csproj version to {0}", modVersion);
}

void CopyDebugArtifactsToAmongUs()
{
    var amongUsPath = GetAmongUsPath();
    var pluginsDir = System.IO.Path.Combine(amongUsPath, "BepInEx", "plugins");
    EnsureDirectoryExists(Directory(pluginsDir));

    var rebuildUsDll = root.CombineWithFilePath("RebuildUs/bin/Debug/net6.0/RebuildUs.dll");
    if (!FileExists(rebuildUsDll))
    {
        throw new Exception("RebuildUs debug DLL was not found: " + rebuildUsDll.FullPath);
    }

    CopyFileForceToDirectory(rebuildUsDll, Directory(pluginsDir));
}

void CopyReleaseArtifactsToAmongUs()
{
    var amongUsPath = GetAmongUsPath();
    var pluginsDir = System.IO.Path.Combine(amongUsPath, "BepInEx", "plugins");
    EnsureDirectoryExists(Directory(pluginsDir));

    var rebuildUsDll = root.CombineWithFilePath("RebuildUs/bin/Release/net6.0/RebuildUs.dll");
    var submergedDll = root.CombineWithFilePath("SDK/Submerged.dll");
    var reactorDll = root.CombineWithFilePath("SDK/Reactor.dll");

    if (!FileExists(rebuildUsDll) || !FileExists(submergedDll) || !FileExists(reactorDll))
    {
        throw new Exception("Release artifacts were not found. Make sure build steps succeeded.");
    }

    CopyFileForceToDirectory(rebuildUsDll, Directory(pluginsDir));
    CopyFileForceToDirectory(submergedDll, Directory(pluginsDir));
    CopyFileForceToDirectory(reactorDll, Directory(pluginsDir));
}

void KillAmongUsProcesses()
{
    var exitCode = RunProcess("taskkill.exe", "/IM \"Among Us.exe\" /F", root);
    Information("taskkill exit code: {0}", exitCode);
}

void StartAmongUsInstances(int count, string gamePath)
{
    var exePath = System.IO.Path.Combine(gamePath, "Among Us.exe");
    if (!System.IO.File.Exists(exePath))
    {
        throw new Exception("Among Us.exe was not found: " + exePath);
    }

    for (var i = 0; i < count; i++)
    {
        RunProcessOrFail(exePath, string.Empty, root);
        System.Threading.Thread.Sleep(2000);
    }
}

void BuildDebugFlow()
{
    BuildReactorReleaseAndCopyToSdk();
    BuildSubmergedAndCopyToSdk("Release");
    BuildRebuildUs("Debug");
    CopyDebugArtifactsToAmongUs();
}

void BuildReleaseFlow()
{
    BuildReactorReleaseAndCopyToSdk();
    BuildSubmergedAndCopyToSdk("Release");
    SyncVersionFromMainToCsproj();
    BuildRebuildUs("Release");
    CopyReleaseArtifactsToAmongUs();
}

Task("BuildReactor")
    .Does(() =>
{
    BuildReactorReleaseAndCopyToSdk();
});

Task("BuildSubmerged")
    .IsDependentOn("BuildReactor")
    .Does(() =>
{
    BuildSubmergedAndCopyToSdk(configuration);
});

Task("BuildDebug")
    .Does(() =>
{
    BuildDebugFlow();
});

Task("BuildRelease")
    .Does(() =>
{
    BuildReleaseFlow();
});

Task("BuildLauncher")
    .Does(() =>
{
    var outputDir = root.Combine("Release/Launcher");
    EnsureDirectoryExists(outputDir);

    RunProcessOrFail(
        "dotnet",
        "publish \"" + launcherProject.FullPath + "\" -c Release -o \"" + outputDir.FullPath + "\" --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false",
        root);

    var whereExitCode = RunProcess("where", "iscc.exe", root);
    if (whereExitCode == 0)
    {
        RunProcessOrFail("iscc.exe", "\"" + launcherIssFile.FullPath + "\"", root);
    }
    else
    {
        Information("Inno Setup Compiler was not found. Installer generation is skipped.");
    }
});

Task("BuildImpostor")
    .Does(() =>
{
    var impostorProject = root.CombineWithFilePath("RebuildUs.Impostor/RebuildUs.Impostor.csproj");
    if (!FileExists(impostorProject))
    {
        throw new Exception("RebuildUs.Impostor project was not found.");
    }

    RunProcessOrFail("dotnet", "build \"" + impostorProject.FullPath + "\" -c Debug", root);

    var serverPath = ReadTrimmedTextFile(serverEnvFile);
    var buildPath = root.Combine("RebuildUs.Impostor/bin/Debug/net8.0");
    var pluginsPath = Directory(System.IO.Path.Combine(serverPath, "plugins"));
    EnsureDirectoryExists(pluginsPath);

    var dlls = GetFiles(buildPath.FullPath + "/*.dll");
    foreach (var dll in dlls)
    {
        var fileName = dll.GetFilename().ToString();
        var toPlugins =
            fileName.Equals("RebuildUs.Impostor.dll", StringComparison.OrdinalIgnoreCase) ||
            fileName.Equals("Newtonsoft.Json.dll", StringComparison.OrdinalIgnoreCase) ||
            fileName.StartsWith("Discord.", StringComparison.OrdinalIgnoreCase);

        if (toPlugins)
        {
            CopyFileForceToDirectory(dll, pluginsPath);
        }
        else
        {
            CopyFileForceToDirectory(dll, Directory(serverPath));
        }
    }
});

Task("PublishUpdater")
    .Does(() =>
{
    RunProcessOrFail("go", "build -o BepInExUpdater/publish/BepInExUpdater.exe BepInExUpdater/main.go", root);
});

Task("GenerateRelease")
    .Does(() =>
{
    if (string.IsNullOrWhiteSpace(version))
    {
        throw new Exception("Please pass --version=... when using GenerateRelease.");
    }

    var mainCsPath = MakeAbsolute(mainCsFile).FullPath;
    var mainContent = System.IO.File.ReadAllText(mainCsPath);
    var updatedMain = System.Text.RegularExpressions.Regex.Replace(
        mainContent,
        "public const string MOD_VERSION = \".*?\";",
        "public const string MOD_VERSION = \"" + version + "\";");
    System.IO.File.WriteAllText(mainCsPath, updatedMain);

    KillAmongUsProcesses();
    BuildReleaseFlow();
    RunProcessOrFail("go", "build -o BepInExUpdater/publish/BepInExUpdater.exe BepInExUpdater/main.go", root);
    RunProcessOrFail("go", "run main.go " + version, root.Combine("ReleaseZipGenerator"));
});

Task("FormatCode")
    .Does(() =>
{
    RunProcessOrFail("dotnet", "format RebuildUs.sln", root);
});

Task("StartDebug")
    .Does(() =>
{
    BuildDebugFlow();
    var gamePath = ReadTrimmedTextFile(debugEnvFile);
    StartAmongUsInstances(1, gamePath);
});

Task("StartDebugBoot")
    .Does(() =>
{
    KillAmongUsProcesses();
    var gamePath = ReadTrimmedTextFile(debugEnvFile);
    StartAmongUsInstances(launchCount, gamePath);
});

Task("Taskkill")
    .Does(() =>
{
    KillAmongUsProcesses();
});

Task("FormatTranslations")
    .Does(() =>
{
    RunProcessOrFail("powershell.exe", "-ExecutionPolicy Bypass -File format-translations.ps1", root);
});

RunTarget(target);