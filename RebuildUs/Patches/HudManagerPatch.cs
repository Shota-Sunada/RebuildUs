namespace RebuildUs.Patches;

[HarmonyPatch]
public static class HudManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static void UpdatePrefix(HudManager __instance)
    {
        CustomOption.HudSettingsManager.UpdateScrollerPosition(__instance);
        CustomOption.HudSettingsManager.UpdateHudSettings(__instance);
        CustomOption.HudSettingsManager.UpdateEndGameSummary(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static void UpdatePostfix(HudManager __instance)
    {
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.CoShowIntro))]
    public static void CoShowIntroPrefix()
    {
    }
}