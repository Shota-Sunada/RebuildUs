namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PingTrackerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static void UpdatePostfix(PingTracker __instance)
    {
        Credits.UpdatePingText(__instance);
    }
}
