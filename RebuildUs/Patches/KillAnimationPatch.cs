namespace RebuildUs.Patches;

[HarmonyPatch]
public static class KillAnimationPatch
{
    public static bool HideNextAnimation = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
    public static void CoPerformKillPrefix(KillAnimation __instance, [HarmonyArgument(0)] ref PlayerControl source, [HarmonyArgument(1)] ref PlayerControl target)
    {
        if (HideNextAnimation)
        {
            source = target;
        }
        HideNextAnimation = false;
    }
}