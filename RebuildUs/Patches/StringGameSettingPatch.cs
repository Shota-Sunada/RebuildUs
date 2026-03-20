namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class StringGameSettingPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringGameSetting), nameof(StringGameSetting.GetValueString))]
    internal static bool GetValueStringPrefix(StringGameSetting __instance, float value, ref string __result)
    {
        return CustomOption.AdjustStringForViewPanel(__instance, value, ref __result);
    }
}