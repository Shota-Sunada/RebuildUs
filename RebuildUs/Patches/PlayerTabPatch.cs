using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerTabPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
    public static void PlayerTabOnEnablePostfix(PlayerTab __instance)
    {
        CustomColors.EnablePlayerTab(__instance);
    }
}