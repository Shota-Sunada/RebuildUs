global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
global using Il2CppInterop.Runtime.Injection;

global using HarmonyLib;
global using Hazel;
global using BepInEx;
global using BepInEx.Logging;
global using BepInEx.Configuration;
global using BepInEx.Unity;
global using BepInEx.Unity.IL2CPP;
global using UnityEngine;
global using TMPro;

using RebuildUs.Options;
using RebuildUs.Modules;

namespace RebuildUs;

[BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
[BepInProcess("Among Us.exe")]
public class RebuildUs : BasePlugin
{
    public const string MOD_ID = "com.shota-sunada.rebuild-us";
    public const string MOD_NAME = "Rebuild Us";
    public const string MOD_VERSION = "1.0.0";
    public const string MOD_DEVELOPER = "Shota Sunada";

    public static RebuildUs Instance;
    public Harmony Harmony { get; } = new(MOD_ID);
    public Version Version { get; } = Version.Parse(MOD_VERSION);

    public static int OptionsPage = 0;

    public override void Load()
    {
        Logger.Initialize(Log);
        Instance = this;

        CustomOptionHolder.Load();
        CustomColors.Load();

        Harmony.PatchAll();

        Logger.LogMessage("\"Rebuild Us\" was completely loaded! Enjoy the modifications!");
    }
}