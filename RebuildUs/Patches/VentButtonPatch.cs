namespace RebuildUs.Patches;

[HarmonyPatch]
public static class VentButtonPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.SetTarget))]
    public static void SetTargetPostfix(VentButton __instance)
    {
        Usables.VentButtonSetTarget(__instance);
    }
}
