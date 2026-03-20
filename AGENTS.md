# RebuildUs AGENTS.md

This file provides guidance for AI agents working on the RebuildUs project.

## Project Overview

RebuildUs is an Among Us mod built with BepInEx, IL2CPP, Harmony, and Reactor. It adds custom roles, game modes, and features to Among Us.

## Environment Requirements

- .NET 6.0 SDK
- PowerShell 5.1+
- Among Us installed (set `$env:AMONG_US` to the game directory)

## Build Commands

### Debug Build
```powershell
./build-debug.ps1
```
Builds the mod in Debug configuration and copies to BepInEx plugins folder.

### Release Build
```powershell
./build-release.ps1
```
Builds the mod in Release configuration and generates distribution files.

### Single File Build
```powershell
dotnet build RebuildUs/RebuildUs.csproj -c Debug
```

### Code Formatting
```powershell
./format.ps1
# or
dotnet format RebuildUs.sln
```

### Clean Build
```powershell
dotnet clean RebuildUs.sln
```

Note: The build scripts require `$env:AMONG_US` environment variable to be set to your Among Us installation directory (e.g., `C:\Program Files\Steam\steamapps\common\Among Us`).

## Code Style Guidelines

### Language & Framework
- Target Framework: .NET 6.0
- Language Version: Latest (C# 12+)
- ImplicitUsings: enabled
- Nullable: enabled
- AllowUnsafeBlocks: true

### Import Conventions
All common namespaces are defined as global usings in `Main.cs`:
- Il2CppInterop (Runtime, Runtime.InteropTypes)
- HarmonyLib
- Hazel
- BepInEx
- UnityEngine
- AmongUs.GameOptions
- RebuildUs.* (internal modules)

### Naming Conventions
- **Constants**: `PascalCase` (e.g., `MOD_VERSION`)
- **Parameters**: `camelCase` (harmony patch parameters can use `__` prefix)
- **Fields/Properties**: `PascalCase`
- **Methods**: `PascalCase`
- **Files**: `{ClassName}Patcher.cs` for Harmony patches

### Brace Style
- **Required** for all: `if`, `for`, `foreach`, `while`, `using`
- Multiline `using` blocks must use braces

### Formatting Rules
- Max line length: 150 characters
- Wrap long lines: enabled
- Parameters wrap style: `CHOP_IF_LONG`
- Attributes on separate lines preferred

## Banned APIs (Enforced by Analyzer)

The following are banned and will cause build errors:

| Banned API | Use Instead |
|------------|-------------|
| `System.Linq` | `foreach`, `GetFastEnumerator()` |
| `obj.Cast<T>()` | `obj.CastFast<T>()` from `Il2CppHelpers` |
| `DestroyableSingleton<T>.Instance` | `FastDestroyableSingleton<T>.Instance` |
| `BepInEx.Logging.ManualLogSource` | `RebuildUs.Logger` |
| `Regex` (in hot paths) | String manipulation |

See `RebuildUs/BannedSymbols.txt` for the complete list.

## Performance Guidelines

### Critical Rules for IL2CPP
1. **No LINQ** in `Update()`, `FixedUpdate()`, or frequently called patches
2. **Use `CastFast<T>()`** instead of `Cast<T>()` for IL2CPP type conversion
3. **Use `FastDestroyableSingleton<T>.Instance`** for singleton access
4. **Cache** Unity components (`Transform`, `Renderer`) in `Awake`/`Start`
5. **Use `StringBuilder`** for complex string building
6. **Avoid allocations** in hot paths - reuse static/cached collections

### Collection Patterns
```csharp
// Use GetFastEnumerator for Il2Cpp lists
foreach (var item in list.GetFastEnumerator()) { }

// For standard List<T>, use standard enumerator
foreach (var item in list) { }
```

## Architecture

### Directory Structure
```
RebuildUs/
├── Patches/          # Harmony patches (hook definitions only)
├── Modules/          # Functional logic and features
├── Roles/            # Role implementations
│   ├── Crewmate/
│   ├── Impostor/
│   ├── Neutral/
│   └── Modifier/
├── Objects/          # Custom game objects
├── Extensions/       # Extension methods
├── Localization/     # Translation files
├── Enums/            # Custom enums
├── Utilities/        # Helper utilities
├── Attributes/       # Custom attributes
└── Main.cs           # Entry point and global usings
```

### Patch Organization
- File naming: `{TargetClass}Patcher.cs`
- Patches must ONLY contain hook definitions
- Move all functional logic to `Modules/` folder

### Role Architecture
- `PlayerRole` - Abstract base for all roles
- `SingleRoleBase<T>` - For unique roles (one per game)
- `MultiRoleBase<T>` - For roles that can have multiple players
- Use `[RegisterRole]` attribute to register roles

## Logging

Use the internal `RebuildUs.Logger` class:

```csharp
Logger.LogInfo("Message", "Tag");
Logger.LogInfo("Player {0} connected", "Network", ["playerName"]);
Logger.LogWarn("Warning", "Tag");
Logger.LogError("Error", "Tag");
Logger.LogDebug("Debug", "Tag");
```

Always use the `tag` parameter to categorize logs.

## Error Handling

- Always check for null on IL2CPP/Unity objects before access
- Use defensive coding patterns
- If build fails with package restore issues, try:
  ```powershell
  dotnet nuget locals all --clear
  ```

## Submerged Compatibility

When using Submerged APIs, always check for presence first:
```csharp
if (SubmergedCompatibility.IsSubmerged)
{
    // Safe to use Submerged APIs
}
```

## Localization

- All source code strings must be English
- Use `English.json` for localization keys
- Keys must be `PascalCase`, unique, and start with uppercase
- Localization is handled via GitLocalize

## Development Notes

- The `Assembly-CSharp*` folders contain decompiled Among Us source - do not edit
- Only edit the `RebuildUs/` project
- Version is synced from `Main.cs` `MOD_VERSION` constant to csproj during release build
