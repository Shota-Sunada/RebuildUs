namespace RebuildUs.Patches.HideAndSeek;

[HarmonyPatch(typeof(HideAndSeekTimerBar))]
internal static class HideAndSeekTimerBarPatch
{
    [HarmonyPatch(typeof(HideAndSeekTimerBar), nameof(HideAndSeekTimerBar.Update))]
    internal static bool UpdatePrefix(HideAndSeekTimerBar __instance)
    {
        return true;
    }

    [HarmonyPatch(typeof(HideAndSeekTimerBar), nameof(HideAndSeekTimerBar.UpdateTimer))]
    internal static bool UpdateTimerPrefix(HideAndSeekTimerBar __instance, float time, float maxTime)
    {
        return true;
    }
}