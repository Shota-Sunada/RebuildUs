namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class VentButtonPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.SetTarget))]
    internal static void SetTargetPostfix(VentButton __instance)
    {
        Usables.VentButtonSetTarget(__instance);
    }
}