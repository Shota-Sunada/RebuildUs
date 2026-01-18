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

    private static int? ColorId = null;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement), typeof(PlayerControl), typeof(bool))]
    public static void SetMovementPrefix(PlayerControl source, bool canMove)
    {
        var color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
        if (Morphing.Exists && source.IsRole(RoleType.Morphing))
        {
            var index = Palette.PlayerColors.IndexOf(color);
            if (index != -1) ColorId = index;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement), typeof(PlayerControl), typeof(bool))]
    public static void Postfix(PlayerControl source, bool canMove)
    {
        if (ColorId.HasValue) source.RawSetColor(ColorId.Value);
        ColorId = null;
    }
}