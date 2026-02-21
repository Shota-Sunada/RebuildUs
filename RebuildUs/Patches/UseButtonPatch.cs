namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class UseButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    internal static bool SetTargetPrefix(UseButton __instance, [HarmonyArgument(0)] IUsable target)
    {
        return Usables.UseButtonSetTarget(__instance, target);
    }
}