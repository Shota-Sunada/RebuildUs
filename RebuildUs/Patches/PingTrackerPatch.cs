namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PingTrackerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    internal static void UpdatePostfix(PingTracker __instance)
    {
        Credits.UpdatePingText(__instance);
    }
}