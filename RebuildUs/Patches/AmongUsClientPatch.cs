using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using InnerNet;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class AmongUsClientPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
    public static void OnGameJoinedPostfix()
    {
        GameStart.VersionSent = false;
        DiscordEmbedManager.UpdateStatus();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static void ExitGamePrefix()
    {
        DiscordModManager.OnQuitGame();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static void OnGameEndPrefix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        EndGameMain.OnGameEndPrefix(ref endGameResult);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static void OnGameEndPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        GameStart.PlayerVersions.Clear();
        EndGameMain.OnGameEndPostfix(__instance, ref endGameResult);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    public static bool CoStartGameHostPrefix(AmongUsClient __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = RoleAssignment.CoStartGameHost(__instance).WrapToIl2Cpp();
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public static void OnPlayerJoinedPostfix(AmongUsClient __instance, ClientData data)
    {
        GameStart.OnPlayerJoined();
        DiscordEmbedManager.UpdateStatus();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    public static void OnPlayerLeftPostfix(AmongUsClient __instance, ClientData data)
    {
        if (data != null)
        {
            GameStart.OnPlayerLeft(data.Id);
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.OwnerId == data.Id);
            if (player != null) DiscordModManager.OnPlayerLeft(player.FriendCode);
        }
        else
        {
            DiscordEmbedManager.UpdateStatus();
        }
    }
}