namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class EndGameManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    internal static void SetEverythingUpPostfix(EndGameManager __instance)
    {
        EndGameMain.SetupEndGameScreen(__instance);
    }
}