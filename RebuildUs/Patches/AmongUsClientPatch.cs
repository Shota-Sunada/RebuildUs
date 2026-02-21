using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppSystem.Collections;
using InnerNet;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class AmongUsClientPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
    internal static void OnGameJoinedPostfix()
    {
        GameStart.VersionSent = false;
        DiscordEmbedManager.UpdateStatus();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    internal static void ExitGamePrefix()
    {
        DiscordModManager.OnQuitGame();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    internal static void OnGameEndPrefix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        EndGameMain.OnGameEndPrefix(ref endGameResult);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    internal static void OnGameEndPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        GameStart.PlayerVersions.Clear();
        EndGameMain.OnGameEndPostfix(__instance, ref endGameResult);
        DiscordModManager.OnGameEnd();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    internal static bool CoStartGameHostPrefix(AmongUsClient __instance, ref IEnumerator __result)
    {
        __result = RoleAssignment.CoStartGameHost(__instance).WrapToIl2Cpp();
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    internal static void OnPlayerJoinedPostfix(AmongUsClient __instance, ClientData data)
    {
        GameStart.OnPlayerJoined();
        DiscordEmbedManager.UpdateStatus();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    internal static void OnPlayerLeftPostfix(AmongUsClient __instance, ClientData data)
    {
        if (data != null)
        {
            GameStart.OnPlayerLeft(data.Id);
            PlayerControl player = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().FirstOrDefault(p => p.OwnerId == data.Id);
            if (player == null) return;
            string id = DiscordModManager.GetIdentifier(player);
            if (id != null) DiscordModManager.OnPlayerLeft(id);
        }
        else
            DiscordEmbedManager.UpdateStatus();
    }
}