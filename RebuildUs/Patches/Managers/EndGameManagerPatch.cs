namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class EndGameManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    internal static bool SetEverythingUpPrefix(EndGameManager __instance)
    {
        EndGameMain.Override(__instance);
        EndGameMain.Postfix(__instance);

        return false;
    }
}