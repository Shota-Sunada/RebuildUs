namespace RebuildUs.Patches;

[HarmonyPatch]
public static class GameOptionsMenuPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    public static void CreateSettingsPostfix(GameOptionsMenu __instance)
    {
        CustomOption.AdaptTaskCount(__instance);
    }
}
