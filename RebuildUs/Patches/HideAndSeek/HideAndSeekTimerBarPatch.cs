namespace RebuildUs.Patches.HideAndSeek;

[HarmonyPatch(typeof(HideAndSeekTimerBar))]
internal static class HideAndSeekTimerBarPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(HideAndSeekTimerBar.Update))]
    internal static bool UpdatePrefix(HideAndSeekTimerBar __instance)
    {
        return TimerBarManager.Update(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(HideAndSeekTimerBar.UpdateTimer))]
    internal static bool UpdateTimerPrefix(HideAndSeekTimerBar __instance, float time, float maxTime)
    {
        return TimerBarManager.UpdateTimer(__instance, time, maxTime);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(HideAndSeekTimerBar.StartFinalHide))]
    internal static bool StartFinalHidePrefix(HideAndSeekTimerBar __instance)
    {
        return TimerBarManager.StartFinalHide(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(HideAndSeekTimerBar.TaskComplete))]
    internal static bool TaskCompletePrefix(HideAndSeekTimerBar __instance)
    {
        return TimerBarManager.TaskComplete(__instance);
    }
}