namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class GameStartManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    internal static void StartPostfix(GameStartManager __instance)
    {
        GameStart.Start(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    internal static void UpdatePrefix(GameStartManager __instance)
    {
        if (!GameData.Instance)
        {
            return; // No instance
        }
        __instance.MinPlayers = 1;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    internal static void UpdatePostfix(GameStartManager __instance)
    {
        GameStart.UpdatePostfix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    internal static bool BeginGamePrefix(GameStartManager __instance)
    {
        return GameStart.BeginGame(__instance);
    }
}