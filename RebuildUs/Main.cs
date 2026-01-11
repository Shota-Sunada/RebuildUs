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

global using RebuildUs;
global using RebuildUs.Extensions;
global using RebuildUs.Localization;
global using RebuildUs.Modules;
global using RebuildUs.Modules.Consoles;
global using RebuildUs.Modules.CustomOptions;
global using RebuildUs.Modules.EndGame;
global using RebuildUs.Modules.RPC;
global using RebuildUs.Options;
global using RebuildUs.Patches;
global using RebuildUs.Players;
global using RebuildUs.Roles;
global using RebuildUs.Utilities;

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

    public System.Random Rnd = new((int)DateTime.Now.Ticks);

    public override void Load()
    {
        Logger.Initialize(Log);
        Instance = this;

        CustomOptionHolder.Load();
        RoleInfo.Load();
        CustomColors.Load();

        Harmony.PatchAll();

        Logger.LogMessage("\"Rebuild Us\" was completely loaded! Enjoy the modifications!");
    }

    public static void ClearAndReloadRoles()
    {

    }

    public static void FixedUpdate(PlayerControl player)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.FixedUpdate());
        // PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.FixedUpdate());
    }

    // TODO: 実装お願いします
    public static void OnMeetingStart()
    {
        PlayerRole.AllRoles.Do(x => x.OnMeetingStart());
        // PlayerModifier.AllModifiers.Do(x => x.OnMeetingStart());

        // GM.resetZoom();
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
        {
            if (p == 1)
            {
                // Camouflager.resetCamouflage();
                // Morphling.resetMorph();
            }
        })));
    }

    public static void OnMeetingEnd()
    {
        PlayerRole.AllRoles.Do(x => x.OnMeetingEnd());
        // PlayerModifier.AllModifiers.Do(x => x.OnMeetingEnd());

        // CustomOverlays.hideInfoOverlay();
        // CustomOverlays.hideRoleOverlay();
        // CustomOverlays.hideBlackBG();
    }

    public static void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
        {
            PlayerRole.AllRoles.Do(x => x.HandleDisconnect(player, reason));
            // PlayerModifier.AllModifiers.Do(x => x.HandleDisconnect(player, reason));

            // Lovers.HandleDisconnect(player, reason);
            // Shifter.HandleDisconnect(player, reason);

            GameHistory.FinalStatuses[player.PlayerId] = EFinalStatus.Disconnected;
        }
    }
}