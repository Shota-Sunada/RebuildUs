namespace RebuildUs.Patches;

[HarmonyPatch(typeof(ProgressTracker))]
internal static class ProgressTrackerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ProgressTracker.Start))]
    internal static bool StartPrefix(ProgressTracker __instance)
    {
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ProgressTracker.FixedUpdate))]
    internal static bool FixedUpdatePostfix(ProgressTracker __instance)
    {
        if (GameModeManager.CurrentGameMode != CustomGamemode.Normal)
        {
            __instance.gameObject.SetActive(false);
        }

        return true;
    }
}