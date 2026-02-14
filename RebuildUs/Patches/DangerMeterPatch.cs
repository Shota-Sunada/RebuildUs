namespace RebuildUs.Patches;

[HarmonyPatch]
public static class DangerMeterPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DangerMeter), nameof(DangerMeter.SetFirstNBarColors))]
    public static void SetFirstNBarColorsPrefix(DangerMeter __instance, ref Color color)
    {
        // if (PlayerControl.LocalPlayer != Tracker.tracker) return;
        if (__instance == HudManager.Instance.DangerMeter) return;

        color = color.SetAlpha(0.5f);
    }
}