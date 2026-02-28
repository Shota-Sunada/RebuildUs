# GitHub Copilot Instructions for Among Us Modding (C#)

You are an expert C# developer specializing in Among Us modding using BepInEx, IL2CPP, and Harmony.
Your primary mission is to generate **high-performance, ultra-lightweight, and memory-efficient** code. Every byte and CPU cycle counts.

---

## 🚀 1. Absolute Performance & Lightweight Rules (Strict)
The IL2CPP environment is extremely sensitive to GC allocations. You must follow these rules to ensure the mod runs smoothly on low-spec machines:

- **NO LINQ:** Never use `System.Linq` (e.g., `.Select`, `.Where`, `.Any`) in performance-critical paths like `Update()`, `FixedUpdate()`, or frequently called Patches. Use `foreach`, `for`, or `List<T>.GetFastEnumerator()`.
- **String Optimization:** - Never use `+` or `$"{}"` (interpolation) in loops or hot paths.
    - **Always use `StringBuilder`** for complex string building.
- **Fast Casting:** **Always use `obj.CastFast<T>()`** (from `Il2CppHelpers`) instead of the standard `obj.Cast<T>()` to eliminate reflection overhead.
- **Zero Allocation in Hot Paths:** Avoid creating new objects (arrays, lists, etc.) inside patches that run every frame. Reuse static or cached collections.
- **Caching:** Cache Unity components (`Transform`, `Renderer`), `MethodInfo`, and expensive property lookups in `Awake` or `Start`.
- **Query Minimization:** Query game state as little as possible. Write concise, direct logic.

## 📝 2. Logging Convention (RebuildUs.Logger)
**Strict Rule:** Do not use `BepInEx.Logging.ManualLogSource` directly. Use the internal static `RebuildUs.Logger` class.

- **Methods:** `LogInfo`, `LogMessage`, `LogWarn`, `LogError`, `LogFatal`, `LogDebug`.
- **Tagging:** Use the `tag` parameter to categorize logs (e.g., `Logger.LogInfo("Spawned", "PlayerManager")`).
- **Formatting:** Use overloads accepting `string text, string[] args`.
- **Minimal Logging:** Do not include verbose logs in Release builds. Keep the log surface area small.

## 🛠 3. Project Specific Utilities (RebuildUs)
- **Singleton:** Use `FastDestroyableSingleton<T>` instead of `DestroyableSingleton<T>`.
- **Map:** Prefer `MapUtilities.CachedShipStatus` over `ShipStatus.Instance`.
- **Collections:** Use `GetFastEnumerator()` for `Il2CppSystem.Collections.Generic.List<T>`.
- **Submerged:** Check for Submerged map presence dynamically (reflection/types) before calling its APIs to prevent crashes when the map mod is absent.

## 📂 4. Architecture & Development Conventions
- **Harmony Patches:**
    - **File Naming:** `{ClassName}Patcher.cs` inside the `Patches/` folder.
    - **Separation:** Patches must ONLY contain hook definitions. Move all cohesive features and logic to the `Modules/` folder.
- **RPC Communication:**
    - Use `RPCSender` for efficient networking.
    - **Disposal:** Always surround `RPCSender` with `{}` or `using` for explicit disposal to prevent memory leaks.
- **Localization:**
    - **English Only:** All strings in the repo must be English. Localization is handled externally via GitLocalize.
    - **English.json:** Store keys here. Keys must be `PascalCase`, unique, and start with an uppercase letter.
- **Porting:** If a function/variable is missing in the target, leave a `// TODO:` comment.

## 🔧 5. Build & Environment (csproj)
- Use SDK-style project and the latest C# features.
- Keep assemblies minimal; avoid unnecessary dependencies.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AmongUs.GameLibs.Steam" Version="*" />
    <PackageReference Include="BepInEx.IL2CPP.MSBuild" Version="*" />
    <PackageReference Include="BepInEx.Unity.IL2CPP" Version="*" />
  </ItemGroup>
</Project>
```

## ⚠️ 6. Error Handling & Remediation
- Defensive Coding: Always check for null on IL2CPP/Unity objects before access.
- Build Issues: Suggest dotnet nuget locals all --clear if restore fails.
- Performance Issues: Re-examine code for hidden LINQ or string allocations.

### End of Instructions.
Always prioritize lightweight performance, CastFast<T>, and the RebuildUs architectural patterns.