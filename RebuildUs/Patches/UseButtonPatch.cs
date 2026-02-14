namespace RebuildUs.Patches;

[HarmonyPatch]
public static class UseButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    public static bool SetTargetPrefix(UseButton __instance, [HarmonyArgument(0)] IUsable target)
    {
        return Usables.UseButtonSetTarget(__instance, target);
    }
}