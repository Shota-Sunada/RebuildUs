namespace RebuildUs.Patches;

[HarmonyPatch]
public static class GameStartManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public static void StartPostfix(GameStartManager __instance)
    {
        GameStart.Start(__instance);
        DiscordEmbedManager.UpdateStatus();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static void UpdatePrefix(GameStartManager __instance)
    {
        if (!GameData.Instance) return; // No instance
        __instance.MinPlayers = 1;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static void UpdatePostfix(GameStartManager __instance)
    {
        GameStart.UpdatePostfix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public static bool BeginGamePrefix(GameStartManager __instance)
    {
        return GameStart.BeginGame(__instance);
    }
}