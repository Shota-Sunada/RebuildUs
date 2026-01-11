using BepInEx.Unity.IL2CPP.Utils.Collections;
using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class AmongUsClientPatch
{
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
        EndGameMain.OnGameEnd(__instance, ref endGameResult);
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
    public static void OnPlayerJoinedPostfix(AmongUsClient __instance)
    {
        GameStart.OnPlayerJoined();
    }
}