namespace RebuildUs.Patches;

[HarmonyPatch]
public static class StringGameSettingPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringGameSetting), nameof(StringGameSetting.GetValueString))]
    public static bool GetValueStringPrefix(StringGameSetting __instance, float value, ref string __result)
    {
        return CustomOption.AdjustStringForViewPanel(__instance, value, ref __result);
    }
}