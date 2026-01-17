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

    private static int? colorId = null;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement))]
    public static void SetMovementPrefix(PlayerControl source, bool canMove)
    {
        var color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
        if (color != null && Morphling.morphling != null && source.Data.PlayerId == Morphling.morphling.PlayerId)
        {
            var index = Palette.PlayerColors.IndexOf(color);
            if (index != -1) colorId = index;
        }
    }

    public static void Postfix(PlayerControl source, bool canMove)
    {
        if (colorId.HasValue) source.RawSetColor(colorId.Value);
        colorId = null;
    }
}