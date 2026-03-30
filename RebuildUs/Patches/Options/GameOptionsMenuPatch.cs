namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class GameOptionsMenuPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    internal static void CreateSettingsPostfix(GameOptionsMenu __instance)
    {
        CustomOption.AdaptTaskCount(__instance);
    }
}