namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PlayerTabPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
    internal static void PlayerTabOnEnablePostfix(PlayerTab __instance)
    {
        CustomColors.EnablePlayerTab(__instance);
    }
}