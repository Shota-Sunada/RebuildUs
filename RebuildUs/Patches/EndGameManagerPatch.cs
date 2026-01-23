using RebuildUs.Modules.Discord;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class EndGameManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static void StartPostfix()
    {
        DiscordModManager.OnGameEnd();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static void SetEverythingUpPostfix(EndGameManager __instance)
    {
        EndGameMain.SetupEndGameScreen(__instance);
    }
}