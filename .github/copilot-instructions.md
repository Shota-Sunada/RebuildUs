## GitHub Instructions for AmongUs Mod (C#)

Purpose
- Instructions and conventions for developing an AmongUs mod using C# in this repository.
- Keep builds lightweight, robust, and maintainable.
- All text below is in English only.
- Use the latest C# language version.

Required NuGet packages
- AmongUs.GameLibs.Steam
  - https://nuget.bepinex.dev/packages/AmongUs.GameLibs.Steam
- BepInEx.IL2CPP.MSBuild
  - https://nuget.bepinex.dev/packages/BepInEx.IL2CPP.MSBuild
- BepInEx.Unity.IL2CPP
  - https://nuget.bepinex.dev/packages/BepInEx.Unity.IL2CPP

Quick start — create project
1. Create an SDK-style C# project. Example minimal csproj snippets below should be adapted to your mod’s needs and target runtime.

2. Add package references to the csproj:
```xml
<ItemGroup>
  <PackageReference Include="AmongUs.GameLibs.Steam" Version="*" />
  <PackageReference Include="BepInEx.IL2CPP.MSBuild" Version="*" />
  <PackageReference Include="BepInEx.Unity.IL2CPP" Version="*" />
</ItemGroup>
```
- Replace "*" with the explicit versions used in CI for reproducible builds.
- If you vendor packages locally, put them in /lib/ and reference via HintPath or LocalPackagePrimary for MSBuild.

Build guidance (lightweight and robust)
- Prefer Release builds for distribution; use Debug for development.
- Keep assemblies minimal: avoid unnecessary dependencies.
- Strip unused code where possible; use IL2CPP linker settings via BepInEx MSBuild package if applicable.
- Use single responsibility classes and small methods to keep code testable and easier to debug.

Typical build commands
- Using dotnet/MSBuild (example):
  - dotnet restore
  - dotnet build -c Release
- If BepInEx IL2CPP MSBuild targets are required, follow the package docs to run the IL2CPP build pipeline; include those targets in your csproj.

Error handling and remediation
- If a build or runtime error occurs:
  1. Read the error message and stack trace fully.
  2. Rebuild with diagnostics enabled:
     - dotnet build -c Release -v:minimal (increase verbosity for details).
  3. Ensure NuGet package versions are compatible with each other and with the game’s binary versions.
  4. If IL2CPP conversion errors occur, check symbol/mapping settings and exclude problematic types from stripping, or update MSBuild IL2CPP options.
  5. Add a minimal repro: isolate the failing change in a small project to reproduce quickly.
  6. If package restore fails, clear caches and restore again:
     - dotnet nuget locals all --clear
     - dotnet restore
  7. Fix code to handle nulls, unexpected types, and version differences. Keep fixes minimal and well-tested.

Development conventions
- Namespace: use your organization or mod name prefix (e.g., YourModName.*).
- Single class per file; small methods; favor composition.
- When applying patches with Harmony, always create a file named `{ClassName}Patcher.cs` in the `Patches` folder and call functions within that file.
- Implementing function logic directly within the `Patches` folder is discouraged. Create functions in their respective module or role implementation files and call them from the `Patches`.
- When sending RPCs, use `RPCSender` as much as possible for smart and efficient communication.
- When using RPCSender, surround the declaration with {} to explicitly dispose of it.
- Strictly ensure weight reduction (lightweight performance and binary size).
- Always use the latest C# language version features and syntax.
- Public API: keep surface area small. Mark internals with InternalsVisibleTo for tests if needed.
- Logging: use BepInEx logging facilities. Log at appropriate levels (Info, Warning, Error). Avoid verbose logs in Release.
- Config: use BepInEx configuration APIs for user settings. Provide sensible defaults.
- Translations: do not include translations in repository. English-only strings in code/resources. Localization will be handled via GitLocalize by users.
  - Provide English text keys and default English values only.
- Tests: prefer light unit tests for pure logic. Mock heavy game interactions.

Runtime integration notes
- Target the game’s architecture and version the mod is intended for.
- Keep IL2CPP compatibility in mind: some reflection techniques differ; use BepInEx helpers and the AmongUs.GameLibs.Steam APIs when possible.
- Ensure the compiled assemblies are placed in the correct BepInEx plugin folder for testing with the game.
- There is Among Us source code in folders starting with "Assembly-CSharp", so please refer to it.

Submerged integration
- If your mod requires integration with Submerged (https://github.com/SubmergedAmongUs/Submerged), add a clear compatibility section in your README and code comments.
- Detect Submerged presence at runtime (e.g., via reflection or presence of Submerged types/assemblies) and gracefully degrade features if absent.
- Avoid hard references that break when Submerged is not present; prefer optional/dynamic checks.
- Coordinate shared APIs (event names, message formats) with Submerged’s current API surface; document version compatibility.

CI recommendations
- Use a CI job to restore, build, and run lightweight unit tests.
- Cache NuGet packages between runs.
- Produce packable artifacts (.dll, zip) on successful Release builds.
- Run static analysis, formatting checks, and minimal tests to catch regressions early.

Packaging and releases
- Produce a single plugin .dll (or minimal set of libs) for distribution.
- Provide a small release ZIP with:
  - plugin .dll
  - optional README and changelog (English only)
  - basic installation instructions
- Keep release files minimal; avoid shipping source unless explicitly requested.

Installation (for users)
- Copy the compiled plugin .dll into the BepInEx plugins folder for the AmongUs installation.
- Follow any Submerged-specific instructions if integration is required.

Troubleshooting checklist (quick)
- Build fails: clear NuGet cache, restore, rebuild with verbosity.
- Missing types at runtime: verify target game version and package compatibility; check IL2CPP conversion settings.
- Crashes on load: reduce plugin surface area, add try/catch around initialization, log exceptions, isolate feature causing crash.
- Localization issues: ensure all user-facing text is English only in repo and keys are stable.

Contributing
- Follow repository coding conventions.
- Open PRs that are focused, small, and include tests or manual steps to validate.
- Use clear commit messages and update CHANGELOG for breaking changes.
- Document any runtime compatibility constraints (game versions, Submerged versions).

License and attribution
- Include the project license at repository root.
- Respect licenses of BepInEx, Submerged, and any third-party libraries.

Example minimal csproj (replace versions and properties to match your environment)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <AssemblyName>YourModName</AssemblyName>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AmongUs.GameLibs.Steam" Version="*" />
    <PackageReference Include="BepInEx.IL2CPP.MSBuild" Version="*" />
    <PackageReference Include="BepInEx.Unity.IL2CPP" Version="*" />
  </ItemGroup>
</Project>
```

Endnote
- Keep builds lightweight, handle errors promptly and conservatively, and provide only English strings in the repository.